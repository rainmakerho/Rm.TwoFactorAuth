# Rm.TwoFactorAuth

An ABP module that adds **TOTP (Time-based One-Time Password) two-factor authentication (MFA)** with:

- Google / Microsoft Authenticator support
- QR Code & manual setup key
- Optional **MFA enforcement middleware**
- Testable Application + Web integration tests

## Compatibility

| Package Version | ABP Version |
|-----------------|-------------|
| 10.0.3          | 10.0.3      |

--- 

## Features

- [x] Enable / Disable / Reset TOTP-based MFA
- [x] Admin can reset MFA for users in User Management
- [x] QR Code provisioning (`otpauth://`)
- [x] Manual setup key (for devices without camera)
- [x] Optional enforcement middleware (force all users to enable MFA)
- [x] Integrates with ABP Account Profile page (`/Account/Manage`)
- [x] **Multi-tenant support** - Issuer and Enforcement settings can be configured per tenant
- [x] Designed for easy mocking & testing

---

## Installation (Web Project)

### 1) Add module dependency

In your **host web module**:

```csharp
[DependsOn(
    typeof(Rm.TwoFactorAuth.Web.TwoFactorAuthWebModule)
)]
public class YourHostWebModule : AbpModule
{
}
```

> No additional pipeline code is required in the host project. The module registers required components.

### 2) Minimal `appsettings.json`

```json
{
  "Settings": {
    "RmTwoFactorAuth.Issuer": "Rm.TwoFactorAuth",
    "RmTwoFactorAuth.Enforcement.Enabled": "false"
  },
  "RmTwoFactorAuth": {
    "Enforcement": {
      "EnrollPath": "/account/manage",
      "ApiReturnUnauthorizedInsteadOfRedirect": true
    }
  }
}
```

#### Settings (ABP Setting System - supports per-tenant configuration)

| Key                                   | Description                                 | Default            |
| ------------------------------------- | ------------------------------------------- | ------------------ |
| `RmTwoFactorAuth.Issuer`              | App name shown in Authenticator apps        | `Rm.TwoFactorAuth` |
| `RmTwoFactorAuth.Enforcement.Enabled` | Force all authenticated users to enable MFA | `false`            |

These settings can be configured:

- Globally via `appsettings.json` under the `Settings` section
- Per-tenant via the Setting Management API or database

#### Options (Static configuration)

| Key                                                  | Description                                       | Default                   |
| ---------------------------------------------------- | ------------------------------------------------- | ------------------------- |
| `Enforcement.EnrollPath`                             | Page users are redirected to when MFA is required | `/account/manage`         |
| `Enforcement.ApiReturnUnauthorizedInsteadOfRedirect` | APIs return 401 instead of redirect               | `true`                    |
| `Enforcement.AllowList`                              | Paths that bypass MFA enforcement                 | Please see the following. |

- Default allowlist typically includes:

```csharp
"/account/login",
"/account/loginwith2fa",
"/account/logout",
"/account/manage",
"/settingmanagement",
"/abp",
"/api/abp",
"/api/rm/two-factor",
"/health",
"/css", "/js", "/lib", "/images", "/favicon", "/assets"
```

### 3) Setting Management API

You can manage settings per-tenant via API:

```http
POST /api/rm/two-factor/setting
Content-Type: application/json

{
  "issuer": "My Company MFA",
  "enforcementEnabled": true
}
```

## User Flow

1. User logs in normally
2. User visits /Account/Manage
3. MFA section is shown:
   - QR Code
   - Manual setup key

4. User scans QR or enters setup key
5. User enters 6-digit verification code
6. MFA is enabled

If enforcement is enabled:

- Non-MFA users are automatically redirected to the enroll page.

### UI Display Notes

- Account Profile (`/Account/Manage`): shows QR code + manual setup key when MFA is not enabled, and shows Disable/Reset actions when enabled.
  ![image](/assets/01.png)

- User Login verify MFA Code (`/Account/LoginWith2fa`): Processes the second stage of the authentication flow. It validates the user-submitted MFA token and establishes a secure session upon successful verification.
  ![image](/assets/02.png)

- Identity Users (`/Identity/Users`): adds a "Reset MFA" action in the user row actions for administrators.
  ![image](/assets/03.png)

  ![image](/assets/04.png)

- Settings Management: Tenant administrators can configure Issuer and Enforcement settings.
  ![image](/assets/06.png)

  ![image](/assets/05.png)

## API Endpoints

| Method | Path                          | Description                    |
| ------ | ----------------------------- | ------------------------------ |
| GET    | `/api/rm/two-factor/setup`    | Returns MFA status             |
| GET    | `/api/rm/two-factor/qr`       | Returns QR code image          |
| POST   | `/api/rm/two-factor/enable`   | Enable MFA                     |
| POST   | `/api/rm/two-factor/disable`  | Disable MFA                    |
| POST   | `/api/rm/two-factor/reset`    | Reset MFA (new key)            |
| POST   | `/api/rm/two-factor/reset-id` | Admin reset MFA by userId      |
| POST   | `/api/rm/two-factor/setting`  | Update current tenant settings |

#### Status Codes

- `204 No Content` – success (controller returns Task)
- `400 Bad Request` – invalid verification code (throws AbpValidationException)
- `401 Unauthorized` – blocked by enforcement middleware in API mode (when enabled)

## Manual Setup Key

For users without a camera:

- A manual setup key is provided
- The Copy button should copy a whitespace-free key
  Example (copy version, no spaces):

```
USS4S 5PCFP NEYUA KGSJE I45PZ CQRG2 Q5
```

## Enforcement Middleware

```csharp
//after app.UseAuthorization();
//using Rm.TwoFactorAuth.Web.Enforcement;
app.UseEnforcementTwoFactorAuth();
```

When enabled:

- All authenticated users must enable MFA
- Allowed paths are configurable via AllowPathPrefixes
- Default allowlist typically includes:

```csharp
"/account/login",
"/account/loginwith2fa",
"/account/logout",
"/account/manage",
"/settingmanagement",
"/abp",
"/api/abp",
"/api/rm/two-factor",
"/health",
"/css", "/js", "/lib", "/images", "/favicon", "/assets"
```

This prevents redirect loops and keeps ABP infrastructure endpoints working.

## Multi-Tenant Support

This module fully supports ABP multi-tenancy:

- **Issuer**: Each tenant can have a different app name shown in Authenticator apps
- **Enforcement**: Each tenant can independently enable/disable MFA enforcement

Settings are stored in the `AbpSettings` table and can be managed via:

1. Setting Management UI
2. Setting Management API (`/api/rm/two-factor/setting`)
3. Direct database update

## NuGet Packages
[![NuGet](https://img.shields.io/nuget/v/Rm.TwoFactorAuth.Web.svg)](https://www.nuget.org/packages/Rm.TwoFactorAuth.Web/)

Install **only** `Rm.TwoFactorAuth.Web`. The other packages are pulled in automatically as dependencies.

## License

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
