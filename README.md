# WebAuth - ASP.NET MVC Application with OWIN

An ASP.NET MVC application using .NET Framework 4.8 with dual authentication support: local cookie-based authentication and Auth0 OpenID Connect.

## Project Structure

```
WebAuth/
├── App_Start/
│   └── RouteConfig.cs          # MVC route configuration
├── Controllers/
│   ├── AccountController.cs    # Authentication controller (Login/Logout)
│   └── HomeController.cs       # Home controller
├── Models/
│   └── LoginViewModel.cs       # Login form model
├── Properties/
│   └── AssemblyInfo.cs         # Assembly metadata
├── Views/
│   ├── Account/
│   │   └── Login.cshtml        # Login page
│   ├── Home/
│   │   └── Index.cshtml        # Home page view
│   ├── Shared/
│   │   └── _Layout.cshtml      # Shared layout with auth navbar
│   ├── _ViewStart.cshtml       # View initialization
│   └── Web.config              # Views configuration
├── Global.asax                 # Application events
├── Global.asax.cs              # Application startup
├── Startup.cs                  # OWIN startup with cookie authentication
├── Web.config                  # Main application configuration
├── packages.config             # NuGet package dependencies
└── WebAuth.csproj              # Project file
```

## Prerequisites

- Visual Studio 2017 or later
- .NET Framework 4.8
- IIS Express (included with Visual Studio)

## Getting Started

1. **Restore NuGet Packages**
   ```
   nuget restore WebAuth.sln
   ```
   Or use Visual Studio's built-in package restore.

2. **Build the Project**
   ```
   msbuild WebAuth.sln
   ```
   Or press `Ctrl+Shift+B` in Visual Studio.

3. **Run the Application**
   - Press `F5` in Visual Studio to run with debugging
   - Or press `Ctrl+F5` to run without debugging
   - The application will start at `https://localhost:44300/`

## Key Features

- **ASP.NET MVC 5.3**: Model-View-Controller architecture
- **Dual Authentication**: Local cookie-based authentication AND Auth0 OpenID Connect
- **OWIN Middleware**: Cookie and OpenID Connect authentication providers
- **Bootstrap 5.3.8**: Modern responsive UI via CDN
- **.NET Framework 4.8**: Latest version of the .NET Framework
- **Razor Views**: Dynamic page rendering with Razor syntax
- **Claims-Based Identity**: Modern authentication with claims
- **Session Storage**: Auth0 ID token claims stored in ASP.NET session (no database required)

## Authentication

This application supports **two authentication methods**:

1. **Local Authentication** - Cookie-based authentication with demo credentials
2. **Auth0 Authentication** - OpenID Connect authentication via Auth0

Users can choose either method on the login page. Both authentication types use the same cookie middleware for session management, with provider information stored in the session.

### Authentication Options

#### Option 1: Local Authentication (Demo)

The application includes hardcoded demo credentials for testing:
- **Username:** `demo`
- **Password:** `password`

**Usage:**
1. Click "Login" in the top-right navbar
2. On the login page, use the "Login with Demo Account" form
3. Enter the demo credentials
4. After successful login, you'll see "Welcome, demo" in the navbar
5. Click "Logout" to end the session

#### Option 2: Auth0 Authentication

**Setup:**
1. Create a free Auth0 account at https://auth0.com
2. Create a new "Regular Web Application"
3. Configure application settings in the Auth0 dashboard:
   - **Allowed Callback URLs**: `https://localhost:44300/signin-auth0`
   - **Allowed Logout URLs**: `https://localhost:44300/`
4. Copy credentials from Auth0 dashboard:
   - Domain (e.g., `your-tenant.auth0.com`)
   - Client ID
   - Client Secret
5. Update `Web.config` with your Auth0 credentials:
   ```xml
   <add key="auth0:Domain" value="your-tenant.auth0.com" />
   <add key="auth0:ClientId" value="YOUR_AUTH0_CLIENT_ID" />
   <add key="auth0:ClientSecret" value="YOUR_AUTH0_CLIENT_SECRET" />
   ```

**Usage:**
1. Click "Login" in the top-right navbar
2. On the login page, click "Login with Auth0"
3. You'll be redirected to Auth0 for authentication
4. After successful login, you'll be redirected back to the application
5. You'll see your Auth0 display name and an "Auth0" badge in the navbar
6. Click "Logout" to end the session (signs out from both the app and Auth0)

### Session Storage

- **Local Authentication**: Stores username and provider type ("local") in session
- **Auth0 Authentication**: Stores name, email, picture, sub (user ID), and provider type ("auth0") in session
- No database required for user storage

### Features

- **Dual Authentication**: Choose between local or Auth0 login
- **Login/Logout**: Full authentication workflow for both providers
- **Secure Cookies**: HttpOnly cookies to prevent XSS attacks
- **Session Management**: 30-minute timeout with sliding expiration
- **Anti-CSRF Protection**: Token validation on all POST requests
- **Return URL Support**: Redirects to original page after login
- **Provider Tracking**: Visual badge indicates Auth0 users in navbar

### Production Migration

The current implementation uses hardcoded credentials for demo purposes. For production:
- Replace hardcoded validation with database lookup
- Use proper password hashing (e.g., `Rfc2898DeriveBytes` with salt)
- Implement account lockout after failed attempts
- Set `CookieSecure` to `Always` (HTTPS only)
- Enable HTTPS enforcement in Web.config

## OWIN Configuration

Dual authentication (Cookie + OpenID Connect) is configured in `Startup.cs`:

```csharp
public void Configuration(IAppBuilder app)
{
    ConfigureAuth(app);
}

private void ConfigureAuth(IAppBuilder app)
{
    // Cookie authentication for both local and Auth0 sessions
    app.UseCookieAuthentication(new CookieAuthenticationOptions
    {
        AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
        CookieName = "WebAuthCookie",
        CookieHttpOnly = true,
        CookieSecure = CookieSecureOption.SameAsRequest,
        ExpireTimeSpan = TimeSpan.FromMinutes(30),
        SlidingExpiration = true,
        LoginPath = new PathString("/Account/Login")
    });

    // OpenID Connect authentication with Auth0
    app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
    {
        AuthenticationType = "Auth0",
        Authority = $"https://{ConfigurationManager.AppSettings["auth0:Domain"]}",
        ClientId = ConfigurationManager.AppSettings["auth0:ClientId"],
        ClientSecret = ConfigurationManager.AppSettings["auth0:ClientSecret"],
        RedirectUri = ConfigurationManager.AppSettings["auth0:RedirectUri"],
        ResponseType = OpenIdConnectResponseType.Code,
        Scope = "openid profile email",
        SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType,

        Notifications = new OpenIdConnectAuthenticationNotifications
        {
            // Store ID token claims in session
            SecurityTokenValidated = notification =>
            {
                var identity = notification.AuthenticationTicket.Identity;
                identity.AddClaim(new Claim("auth_provider", "auth0"));

                // Store claims in ASP.NET session
                var session = HttpContext.Current.Session;
                session["user:name"] = identity.FindFirst("name")?.Value;
                session["user:email"] = identity.FindFirst("email")?.Value;
                session["user:provider"] = "auth0";

                return Task.FromResult(0);
            }
        }
    });
}
```

**Key Points:**
- Cookie middleware handles sessions for both local and Auth0 users
- OpenID Connect middleware handles Auth0 authentication flow
- `SignInAsAuthenticationType` connects OIDC to cookie authentication
- Auth0 ID token claims are stored in ASP.NET session (no database required)

## NuGet Packages Included

### Core MVC
- Microsoft.AspNet.Mvc (5.3.0)
- Microsoft.AspNet.Razor (3.3.0)
- Microsoft.AspNet.WebPages (3.3.0)

### Authentication (OWIN)
- Microsoft.Owin (4.2.3)
- Microsoft.Owin.Host.SystemWeb (4.2.3)
- **Microsoft.Owin.Security (4.2.3)**
- **Microsoft.Owin.Security.Cookies (4.2.3)**
- **Microsoft.Owin.Security.OpenIdConnect (4.2.2)** - Auth0 integration
- **System.IdentityModel.Tokens.Jwt (5.6.0)** - JWT token handling
- **Microsoft.IdentityModel.Protocols.OpenIdConnect (5.6.0)** - OIDC protocol
- **Microsoft.IdentityModel.Tokens (5.6.0)** - Token validation
- Owin (1.0)

### Other
- Microsoft.CodeDom.Providers.DotNetCompilerPlatform (4.1.0)
- Newtonsoft.Json (13.0.4)
- Microsoft.AspNet.Web.Optimization (1.1.3)
- WebGrease (1.6.0)
- Antlr (3.5.0.2)
- Microsoft.Web.Infrastructure (2.0.0)

## Project Architecture

### Controllers

- **AccountController**: Handles login/logout operations for both auth providers
  - `Login()` GET/POST - Displays login form and processes local credentials
  - `LoginAuth0()` - Initiates Auth0 authentication challenge
  - `Auth0Callback()` - Handles Auth0 callback after authentication
  - `Logout()` - Signs out user from both cookie and Auth0 (if applicable)
  - `SignInUser()` - Helper method for local authentication with provider tracking
  - Uses claims-based identity with OWIN authentication manager

- **HomeController**: Displays home page

### Models

- **LoginViewModel**: Login form data model for local authentication
  - Username (required)
  - Password (required)
  - RememberMe (optional)

### Views

- **Login.cshtml**: Bootstrap 5 styled login form with both authentication options
  - Local login form with demo credentials
  - Auth0 login button
- **_Layout.cshtml**: Master layout with authentication-aware navbar
  - Shows user's display name
  - Shows "Auth0" badge for Auth0 users
  - Displays provider-appropriate logout button
- **Index.cshtml**: Home page

## License

This is a scaffolded project template. Add your own license as needed.
