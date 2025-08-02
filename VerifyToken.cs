using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net;

namespace SecureLoginBackend;

public class VerifyToken
{
    private readonly ILogger<VerifyToken> _logger;

    public VerifyToken(ILogger<VerifyToken> logger)
    {
        _logger = logger;
    }

    [Function("VerifyToken")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]
        Microsoft.Azure.Functions.Worker.Http.HttpRequestData req,
        FunctionContext context)
    {
        var logger = context.GetLogger("VerifyToken");

        // Get Authorization header
        var authHeader = req.Headers.GetValues("Authorization").FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            var badTokenResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            await badTokenResponse.WriteStringAsync("Missing or invalid Authorization header.");
            return badTokenResponse;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        // Load validation config from environment
        var issuer = Environment.GetEnvironmentVariable("OpenIdConnect_Issuer");
        var audience = Environment.GetEnvironmentVariable("OpenIdConnect_Audience");

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKeys = await GetSigningKeysFromEntraAsync()
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParams, out _);
            var email = principal.FindFirst(ClaimTypes.Email)?.Value
                        ?? principal.FindFirst("emails")?.Value
                        ?? principal.Identity?.Name;

            var successResponse = req.CreateResponse(HttpStatusCode.OK);
            await successResponse.WriteStringAsync($"Authenticated as {email}");
            return successResponse;
        }
        catch (Exception ex)
        {
            logger.LogError($"Token validation failed: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            await errorResponse.WriteStringAsync("Token validation failed.");
            return errorResponse;
        }
    }

    private static async Task<IEnumerable<SecurityKey>> GetSigningKeysFromEntraAsync()
    {
        var issuerUrl = Environment.GetEnvironmentVariable("OpenIdConnect_IssuerUrl");
        if (string.IsNullOrWhiteSpace(issuerUrl))
        {
            throw new InvalidOperationException("Missing environment variable: OpenIdConnect_IssuerUrl");
        }

        var metadataEndpoint = $"{issuerUrl}/.well-known/openid-configuration";

        var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            metadataEndpoint,
            new OpenIdConnectConfigurationRetriever());

        var config = await configManager.GetConfigurationAsync();
        return config.SigningKeys;
    }
}
