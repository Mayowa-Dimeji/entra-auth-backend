# 🔐 Entra Auth Backend

This is a secure, modular backend built with **Azure Functions (.NET 8 isolated)** for handling user authentication and token verification using **Microsoft Entra External ID** (formerly Azure AD B2C). It is designed to serve as the foundation for a modern login/registration system in a cross-platform app (e.g., Blazor, React, or mobile frontend).

---

## 🎯 Purpose

This project is part of my effort to build a **production-level authentication flow** using technologies widely adopted by modern cloud-native systems, including those used by Perceptor. The goal is to implement:

- 🔒 Secure sign-up and sign-in flows
- ✅ JWT validation from Entra ID
- 🧠 Token-based access control
- 📸 Optional biometric or pattern recognition integration (future)
- 🚀 Scalable, serverless architecture ready for real-world use

---

## 🛠️ Tech Stack

| Layer          | Technology                                     |
| -------------- | ---------------------------------------------- |
| Language       | C# (.NET 8)                                    |
| Backend        | Azure Functions (Isolated Worker Model)        |
| Auth Provider  | Microsoft Entra External ID (OAuth2 + OIDC)    |
| Token Handling | JWT (System.IdentityModel.Tokens.Jwt)          |
| Config Fetch   | OpenID Connect metadata discovery              |
| Hosting        | Azure + Local (for development)                |
| Logging        | ILogger / Azure Application Insights (planned) |
| Secrets        | Azure Key Vault (planned)                      |
| Storage        | Azure Table Storage or CosmosDB (planned)      |

---

## 📦 Features

- ✅ **Secure token validation endpoint** `/api/VerifyToken`
- 🔄 **Live key discovery** from Entra’s JWKS endpoint
- 🌐 Supports **email/password**, **Google**, **Facebook**, and other identity providers via Entra External ID
- 📄 Environment-driven configuration using `local.settings.json`
- 🔌 Ready for integration with any frontend via standard `Authorization: Bearer <token>` header
- 📁 Modular structure for easily adding:
  - `/api/RegisterUser`
  - `/api/LoginUser`

---

## 📍 Status

- [x] ✅ JWT validation logic implemented
- [x] ✅ Entra config environment integration
- [ ] 🔜 User registration & login endpoints
- [ ] 🔐 OAuth flow management (if needed)
- [ ] 📸 Pattern/face recognition module
- [ ] 📊 Logging, monitoring, and metrics

---

## 🚧 Setup & Run Locally

1. Clone the repo

   ```bash
   git clone https://github.com/Mayowa-Dimeji/entra-auth-backend.git
   cd entra-auth-backend
   ```

2. Configure your `local.settings.json`:

   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
       "AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "OpenIdConnect_Issuer": "https://login.microsoftonline.com/<tenant-id>/v2.0",
       "OpenIdConnect_IssuerUrl": "https://login.microsoftonline.com/<tenant-id>/v2.0",
       "OpenIdConnect_Audience": "api://<client-id>"
     }
   }
   ```

3. Run locally:

   ```bash
   func start
   ```

4. Test `/api/VerifyToken` using Postman or curl:
   ```bash
   curl -H "Authorization: Bearer <your_token>" http://localhost:7071/api/VerifyToken
   ```

---

> This repo demonstrates my understanding of secure authentication systems, Azure identity management, and scalable backend design using .NET and Azure-native tooling.

---

## 📬 Contact

- LinkedIn: [linkedin.com/in/mayowaoladimeji](https://www.linkedin.com/in/mayowa-oladimeji/)
- Portfolio: [mayowaoladimeji](https://my-portfolio-gamma-bay-50.vercel.app/) _(placeholder if not live yet)_

---

## 📝 License

MIT — use freely, fork openly, contribute responsibly.
