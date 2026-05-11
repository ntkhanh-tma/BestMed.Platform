# BESTmed Identity Server — Integration Guide for AI Agents

## PURPOSE

This document provides exact instructions for an AI agent to integrate a client service with the BESTmed Identity Server. It covers every grant type, endpoint, request/response format, custom claim, and error code. Follow these instructions literally.

---

## ISSUER URL

The Identity Server base URL (issuer) is environment-dependent. All endpoints below are relative to this URL.

```
Discovery document: {issuer}/.well-known/openid-configuration
```

---

## ENDPOINTS

| Endpoint | URL |
|---|---|
| Authorization | `{issuer}/connect/authorize` |
| Token | `{issuer}/connect/token` |
| UserInfo | `{issuer}/connect/userinfo` |
| End Session (Logout) | `{issuer}/connect/endsession` |
| Discovery | `{issuer}/.well-known/openid-configuration` |
| JWKS (signing keys) | `{issuer}/.well-known/openid-configuration/jwks` |

---

## REGISTERED CLIENTS

Each client below is pre-configured. Use the matching `client_id` for your application.

### Browser-based clients (Authorization Code + PKCE)

| client_id | app | requires_secret | allowed_scopes | access_token_lifetime |
|---|---|---|---|---|
| `bestmed.connect` | Consumer Portal (Angular SPA) | no | `openid profile email api` | 900s (15min) |
| `bestmed.facility` | BESTdose (Angular SPA) | no | `openid profile email api` | 900s |
| `bestmed.tracking` | BESTtrack (Angular SPA) | no | `openid profile email api` | 900s |
| `bestmed.prescribe` | BESTdoctor (Angular SPA) | no | `openid profile email api` | 900s |
| `bestmed.homecare` | HomeCare Web (Angular SPA) | no | `openid profile email api` | 43200s (12hr) |
| `bestmed.app` | BESTmed Offline App (native) | no | `openid profile email offline_access api` | 604800s (7d) |

### Machine-to-machine clients (Client Credentials)

| client_id | requires_secret | allowed_scopes | access_token_lifetime |
|---|---|---|---|
| `bestmed.servers` | yes | `api besmedservers bestpackapi bestdoseapi` | 900s |
| `bestmed.pharmacy` | yes | `api` | 900s |
| `bestmed.homecare.azure` | yes | `api` | 900s |

### Mobile client (Resource Owner Password)

| client_id | requires_secret | allowed_scopes | access_token_lifetime |
|---|---|---|---|
| `bestmed.homecare.mobile` | no | `api` | 43200s (12hr) |

---

## GRANT TYPE 1: AUTHORIZATION CODE + PKCE (Browser / Native Apps)

Use this for any interactive user login.

### Step 1 — Redirect user to authorize endpoint

```http
GET {issuer}/connect/authorize
  ?response_type=code
  &client_id={client_id}
  &redirect_uri={your_registered_redirect_uri}
  &scope=openid profile email api
  &state={random_string}
  &code_challenge={base64url_sha256_of_verifier}
  &code_challenge_method=S256
```

REQUIRED parameters: `response_type`, `client_id`, `redirect_uri`, `scope`, `code_challenge`, `code_challenge_method`.

The server displays a login page. After successful login, it redirects to:

```
{redirect_uri}?code={authorization_code}&state={state}
```

### Step 2 — Exchange code for tokens

```http
POST {issuer}/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=authorization_code
&client_id={client_id}
&code={authorization_code}
&redirect_uri={same_redirect_uri}
&code_verifier={pkce_verifier}
```

NOTE: These clients do NOT require a `client_secret`.

### Response (success — HTTP 200)

```json
{
  "access_token": "eyJ...",
  "id_token": "eyJ...",
  "token_type": "Bearer",
  "expires_in": 900,
  "refresh_token": "abc..."
}
```

`refresh_token` is only returned if the client has `AllowOfflineAccess: true` and `offline_access` is in the requested scope.

### Step 3 — Refresh token (if available)

```http
POST {issuer}/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=refresh_token
&client_id={client_id}
&refresh_token={refresh_token}
```

Refresh tokens use sliding expiration. The sliding window varies per client (see `SlidingRefreshTokenLifetime` in the client table above).

---

## GRANT TYPE 2: CLIENT CREDENTIALS (Server-to-Server)

Use this for backend service calls where no user is involved.

```http
POST {issuer}/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id={client_id}
&client_secret={client_secret}
&scope={space_separated_scopes}
```

### Response (success — HTTP 200)

```json
{
  "access_token": "eyJ...",
  "token_type": "Bearer",
  "expires_in": 900
}
```

### Optional: Attach user context to a client_credentials token

Pass these additional form parameters to embed user claims in the token:

| Parameter | Required | Description |
|---|---|---|
| `bm_uid` | yes | User GUID to look up in the database |
| `bm_id` | no | Token correlation ID (a new GUID is generated if omitted) |

The resulting token will contain `bm_id`, `bm_uid`, `bm_utype`, and `bm_role` as claims.

Example:

```http
POST {issuer}/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id=bestmed.servers
&client_secret={secret}
&scope=api besmedservers
&bm_uid=3fa85f64-5717-4562-b3fc-2c963f66afa6
```

---

## GRANT TYPE 3: RESOURCE OWNER PASSWORD (HomeCare Mobile only)

RESTRICTED to `client_id=bestmed.homecare.mobile`. Do not use for new integrations.

```http
POST {issuer}/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password
&client_id=bestmed.homecare.mobile
&username={username}
&password={password}
&scope=api
```

### Response (success — HTTP 200)

```json
{
  "access_token": "eyJ...",
  "token_type": "Bearer",
  "expires_in": 43200,
  "PasswordExpiredDayLeft": 145
}
```

`PasswordExpiredDayLeft` is a custom field. Values:
- Positive: days remaining before password expires.
- `<= 7`: warn the user to change password soon.
- `0` or negative: password has expired; force a password change.

### Response (error — HTTP 400)

```json
{
  "error": "invalid_grant",
  "error_description": "..."
}
```

See ERROR REFERENCE below for all possible `error_description` values.

---

## CUSTOM CLAIMS IN ACCESS TOKEN AND ID TOKEN

After decoding the JWT, expect these custom claims (in addition to standard `sub`, `name`, `email`, etc.):

| Claim key | Type | Example value | Description |
|---|---|---|---|
| `bm_uid` | string | `"3fa85f64-5717-4562-b3fc-2c963f66afa6"` | User's unique ID |
| `bm_utype` | string | `"Facility"` | User type. One of: `Facility`, `Warehouse`, `Consumer`, `Home Care`, `Pharmacy`, `AHService`, `BESTtrack`, `BESTmedConnect`, `System` |
| `bm_name` | string | `"john.doe"` | Username |
| `bm_firstname` | string | `"John"` | First name |
| `bm_lastname` | string | `"Doe"` | Last name |
| `bm_role` | string | `"6b76364a-af6d-4e44-8e90-7c690e3eeacf"` | Role GUID |

### Client Credentials tokens (when `bm_uid` parameter was sent)

Additional claims:

| Claim key | Description |
|---|---|
| `bm_id` | Token correlation GUID |
| `bm_uid` | User GUID |
| `bm_utype` | User type |
| `bm_role` | Role GUID |

NOTE: `bm_firstname`, `bm_lastname`, `bm_name` are NOT included in client_credentials tokens.

---

## CALLING PROTECTED APIs WITH THE TOKEN

```http
GET {api_base_url}/your/endpoint
Authorization: Bearer {access_token}
```

The downstream API validates the token by:
1. Fetching the JWKS from `{issuer}/.well-known/openid-configuration/jwks`.
2. Verifying the JWT signature.
3. Checking `iss`, `aud`, `exp` claims.
4. Reading `bm_uid`, `bm_utype`, `bm_role` for authorization decisions.

---

## LOGOUT

### Browser logout

Redirect the user to:

```
{issuer}/connect/endsession
  ?id_token_hint={id_token}
  &post_logout_redirect_uri={your_registered_post_logout_uri}
```

### API-only (no user redirect)

Discard the access token and refresh token on the client side. Tokens are self-contained JWTs; there is no server-side revocation endpoint configured.

---

## LOGIN FLOW SIDE EFFECTS (Authorization Code only)

During interactive login, the server may redirect the user through additional pages BEFORE returning the authorization code. The consuming application does NOT need to handle these — they happen inside the Identity Server UI. But be aware of them:

| Condition | Server action | User must |
|---|---|---|
| First login (`IsBESTmedLogin == false`) | Redirect to `/Account/ChangePassword` | Set a new password |
| Terms not accepted | Redirect to `/TermAndCondition` | Accept terms |
| 2FA required, not set up | Redirect to `/Account/LoginWith2FASetupRequired` | Configure authenticator app |
| 2FA required, already set up | Redirect to `/Account/LoginWith2FASetupVerify` | Enter TOTP code |

After completing these steps, the authorization code is issued normally.

---

## ACCESS CONTROL MATRIX

The server enforces these rules. Login attempts that violate them are rejected.

### Allowed user types per client

| client_id | Allowed `bm_utype` |
|---|---|
| `bestmed` | `Facility`, `Warehouse` |
| `bestmed.app` | `Facility` |
| `bestmed.connect` | `Consumer` |
| `bestmed.homecare` | `Home Care` |
| `bestmed.homecare.mobile` | `Home Care` |

### Allowed roles for restricted clients

| client_id | Allowed role GUIDs |
|---|---|
| `bestmed.app` | `6b76364a-af6d-4e44-8e90-7c690e3eeacf` (Management), `b2026815-0d5f-438e-8963-097d88cf9b94` (Administration) |
| `bestmed.homecare` | `1dcf63cb-dd20-4723-9f62-02396ee7f260` (Carer), `d3de087b-1823-4ae3-89f0-966f6582fb01` (EnrolledNurse), `ca12db69-5570-47a2-96c2-f926cef13df3` (Management), `0cf2351f-22b1-4439-bc33-15256db1562e` (RegisteredNurse) |
| `bestmed.homecare.mobile` | Same as `bestmed.homecare` |
| All others | No role restriction |

---

## ACCOUNT LOCKOUT

- After **5** consecutive failed password attempts, the account is locked.
- Web clients: lockout lasts 5 minutes (auto-unlock via `LockoutEnd`), but status is set to `Locked` which may require manual unlock depending on client.
- ROPC (mobile): account status set to `Locked`; must be unlocked by a HomeCare Manager.

---

## PASSWORD POLICY

Passwords must meet ALL of:
- Minimum 8 characters
- At least 1 uppercase letter (A-Z)
- At least 1 lowercase letter (a-z)
- At least 1 digit (0-9)
- At least 1 non-alphanumeric character

---

## ERROR REFERENCE

### Token endpoint errors (ROPC — `bestmed.homecare.mobile`)

| `error` | `error_description` | Meaning |
|---|---|---|
| `invalid_client` | `login failed` | `client_id` is not `bestmed.homecare.mobile` |
| `invalid_grant` | `Invalid Username or Password. Please enter a valid Username and Password.` | User not found, deleted, or role not permitted |
| `invalid_grant` | `User account is locked. Contact your Home Care Manager to unlock.` | Account status is `Locked` |
| `invalid_grant` | `User status is inactive. Contact your Home Care Manager to activate your user account.` | Account is `InActive` or `Suspended` |
| `invalid_grant` | `Unauthorised Access. Please ensure you are connected to the facility network that is registered with BESTMED. If the problem persists, please contact BHS Support.` | IP/geolocation check failed |
| `invalid_grant` | `You do not have access to any facilities. Please contact your facility manager and request to be given access.` | No facility with HomeCare enabled |

### Token endpoint errors (Client Credentials)

Standard OAuth2 errors apply. The most common:

| `error` | Meaning |
|---|---|
| `invalid_client` | Wrong `client_id` or `client_secret` |
| `invalid_scope` | Requested scope not allowed for this client |

### Authorization Code flow errors

Errors are returned as query parameters on the redirect URI:

```
{redirect_uri}?error={error_code}&error_description={message}
```

Or displayed on the Identity Server error page if no valid redirect URI exists.

---

## IMPLEMENTATION CHECKLIST FOR AI AGENTS

When integrating a new service, do the following in order:

1. **Determine the grant type** from the client table above based on your `client_id`.
2. **For Authorization Code**: Implement PKCE (S256). Generate a random `code_verifier` (43-128 chars, unreserved URI chars), SHA-256 hash it, base64url-encode for `code_challenge`.
3. **For Client Credentials**: Store `client_secret` securely (environment variable, key vault). Never embed in frontend code.
4. **Register redirect URIs**: Your `redirect_uri` and `post_logout_redirect_uri` MUST exactly match one of the values configured for your client in `appsettings.json`. Contact the Identity Server admin to add new URIs.
5. **Request only needed scopes**: Use the minimum scopes from the allowed list.
6. **Validate tokens**: Fetch JWKS from the discovery endpoint. Validate `iss`, `aud`, `exp`, and signature on every request.
7. **Read custom claims**: Extract `bm_uid`, `bm_utype`, `bm_role` from the decoded JWT for authorization logic.
8. **Handle token expiry**: If you have a `refresh_token`, use the refresh grant. Otherwise, re-authenticate.
9. **Handle errors**: Parse the `error` and `error_description` fields from failed token responses.
10. **CORS**: If your SPA origin is not in `AllowedCorsOrigins` for your client, token requests from the browser will fail. Contact the Identity Server admin to add your origin.

---

## QUICK-START CODE EXAMPLES

### Authorization Code + PKCE (JavaScript / TypeScript SPA)

```typescript
// Using oidc-client-ts library
import { UserManager } from "oidc-client-ts";

const userManager = new UserManager({
  authority: "{issuer}",
  client_id: "bestmed.connect",
  redirect_uri: "https://your-app.com/index.html",
  post_logout_redirect_uri: "https://your-app.com/index.html",
  response_type: "code",
  scope: "openid profile email api",
});

// Login
await userManager.signinRedirect();

// Callback (on redirect_uri page)
const user = await userManager.signinRedirectCallback();
const accessToken = user.access_token;
// Use accessToken in Authorization header

// Logout
await userManager.signoutRedirect();
```

### Client Credentials (C# / .NET)

```csharp
using var client = new HttpClient();
var response = await client.PostAsync("{issuer}/connect/token",
    new FormUrlEncodedContent(new Dictionary<string, string>
    {
        ["grant_type"] = "client_credentials",
        ["client_id"] = "bestmed.servers",
        ["client_secret"] = "{secret}",
        ["scope"] = "api besmedservers"
    }));

var json = await response.Content.ReadFromJsonAsync<JsonElement>();
var accessToken = json.GetProperty("access_token").GetString();
```

### Client Credentials with user context (C# / .NET)

```csharp
var response = await client.PostAsync("{issuer}/connect/token",
    new FormUrlEncodedContent(new Dictionary<string, string>
    {
        ["grant_type"] = "client_credentials",
        ["client_id"] = "bestmed.servers",
        ["client_secret"] = "{secret}",
        ["scope"] = "api besmedservers",
        ["bm_uid"] = "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }));
```

### Resource Owner Password (C# / .NET — HomeCare Mobile only)

```csharp
var response = await client.PostAsync("{issuer}/connect/token",
    new FormUrlEncodedContent(new Dictionary<string, string>
    {
        ["grant_type"] = "password",
        ["client_id"] = "bestmed.homecare.mobile",
        ["username"] = "john.doe",
        ["password"] = "P@ssw0rd!",
        ["scope"] = "api"
    }));
```

### Validating access tokens in a .NET API

```csharp
// In Program.cs or Startup
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "{issuer}";
        options.Audience = "api";
        options.TokenValidationParameters.ValidateAudience = true;
    });

// In a controller or minimal API
app.MapGet("/protected", (ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue("bm_uid");
    var userType = user.FindFirstValue("bm_utype");
    var role = user.FindFirstValue("bm_role");
    return Results.Ok(new { userId, userType, role });
}).RequireAuthorization();
```

---

## REVISION

Generated from source code in repository `BESTHealthSolut/BestmedIdentify`, branch `main`.
