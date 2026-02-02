# Rm.TwoFactorAuth

An ABP module that adds **TOTP (Time-based One-Time Password) two-factor authentication (MFA)** with:

- Google / Microsoft Authenticator support
- QR Code & manual setup key
- Optional **MFA enforcement middleware**
- Testable Application + Web integration tests

---

## Features

- [x] Enable / Disable / Reset TOTP-based MFA
- [x] Admin can reset MFA for users in User Management
- [x] QR Code provisioning (`otpauth://`)
- [x] Manual setup key (for devices without camera)
- [x] Optional enforcement middleware (force all users to enable MFA)
- [x] Integrates with ABP Account Profile page (`/Account/Manage`)
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
  "RmTwoFactorAuth": {
    "Issuer": "Rm.TwoFactorAuth",
    "Enforcement": {
      "Enabled": true,
      "EnrollPath": "/account/manage",
      "ApiReturnUnauthorizedInsteadOfRedirect": true
    }
  }
}
```

#### Options

| Key                                      | Description                                       |
| ---------------------------------------- | ------------------------------------------------- |
| `Issuer`                                 | App name shown in Authenticator apps              |
| `Enforcement.Enabled`                    | Force all authenticated users to enable MFA       |
| `EnrollPath`                             | Page users are redirected to when MFA is required |
| `ApiReturnUnauthorizedInsteadOfRedirect` | APIs return 401 instead of redirect               |

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

## UI Integration

This module integrates with the ABP Account Profile page by bundling a script into the `Manage` page:

```csharp
Configure<AbpBundlingOptions>(options =>
{
    options.ScriptBundles.Configure(
        typeof(ManageModel).FullName,
        configuration =>
        {
            configuration.AddFiles("/Pages/Account/Components/ProfileManagementGroup/TwoFactorAuthentication/Default.js");
        });
});
```

### UI Display Notes

- Account Profile (`/Account/Manage`): shows QR code + manual setup key when MFA is not enabled, and shows Disable/Reset actions when enabled.

![image](/assets/01.png)

- User Login verify MFA Code (`/Account/LoginWith2fa`): Processes the second stage of the authentication flow. It validates the user-submitted MFA token and establishes a secure session upon successful verification.

![image](/assets/02.png)

- Identity Users (`/Identity/Users`): adds a "Reset MFA" action in the user row actions for administrators.

![image](/assets/03.png)

![image](/assets/04.png)


## API Endpoints

| Method | Path                         | Description           |
| ------ | ---------------------------- | --------------------- |
| GET    | `/api/rm/two-factor/setup`   | Returns MFA status    |
| GET    | `/api/rm/two-factor/qr`      | Returns QR code image |
| POST   | `/api/rm/two-factor/enable`  | Enable MFA            |
| POST   | `/api/rm/two-factor/disable` | Disable MFA           |
| POST   | `/api/rm/two-factor/reset`   | Reset MFA (new key)   |
| POST   | `/api/rm/two-factor/reset-id`| Admin reset MFA by userId |

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
  - `/account/login`
  - `/account/manage`
  - `/api/rm/two-factor`
  - `/api/abp`
  - Static assets (`/css`, `/js`, ...)

This prevents redirect loops and keeps ABP infrastructure endpoints working.

## NuGet Packages

Install **only** `Rm.TwoFactorAuth.Web`. The other packages are pulled in automatically as dependencies.

## License

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
