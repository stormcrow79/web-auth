# WebAuth - ASP.NET MVC Application with OWIN

An ASP.NET MVC application using .NET Framework 4.8 with OWIN cookie-based authentication.

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
- **OWIN Cookie Authentication**: Secure cookie-based authentication
- **Bootstrap 5.3.8**: Modern responsive UI via CDN
- **.NET Framework 4.8**: Latest version of the .NET Framework
- **Razor Views**: Dynamic page rendering with Razor syntax
- **Claims-Based Identity**: Modern authentication with claims

## Authentication

### Demo Credentials

The application includes hardcoded demo credentials for testing:
- **Username:** `demo`
- **Password:** `password`

### Features

- **Login/Logout**: Full authentication workflow
- **Secure Cookies**: HttpOnly cookies to prevent XSS attacks
- **Session Management**: 30-minute timeout with sliding expiration
- **Anti-CSRF Protection**: Token validation on all POST requests
- **Return URL Support**: Redirects to original page after login

### Usage

1. Click "Login" in the top-right navbar
2. Enter the demo credentials
3. After successful login, you'll see "Welcome, demo" in the navbar
4. Click "Logout" to end the session

### Production Migration

The current implementation uses hardcoded credentials for demo purposes. For production:
- Replace hardcoded validation with database lookup
- Use proper password hashing (e.g., `Rfc2898DeriveBytes` with salt)
- Implement account lockout after failed attempts
- Set `CookieSecure` to `Always` (HTTPS only)
- Enable HTTPS enforcement in Web.config

## OWIN Configuration

Cookie authentication is configured in `Startup.cs`:

```csharp
public void Configuration(IAppBuilder app)
{
    ConfigureAuth(app);
}

private void ConfigureAuth(IAppBuilder app)
{
    app.UseCookieAuthentication(new CookieAuthenticationOptions
    {
        AuthenticationType = "ApplicationCookie",
        LoginPath = new PathString("/Account/Login"),
        LogoutPath = new PathString("/Account/Logout"),
        ExpireTimeSpan = TimeSpan.FromMinutes(30),
        SlidingExpiration = true,
        CookieName = "WebAuthCookie",
        CookieHttpOnly = true,
        CookieSecure = CookieSecureOption.SameAsRequest
    });
}
```

## NuGet Packages Included

- Microsoft.AspNet.Mvc (5.3.0)
- Microsoft.AspNet.Razor (3.3.0)
- Microsoft.AspNet.WebPages (3.3.0)
- Microsoft.Owin (4.2.3)
- Microsoft.Owin.Host.SystemWeb (4.2.3)
- **Microsoft.Owin.Security (4.2.3)**
- **Microsoft.Owin.Security.Cookies (4.2.3)**
- Microsoft.CodeDom.Providers.DotNetCompilerPlatform (4.1.0)
- Owin (1.0)
- Newtonsoft.Json (13.0.4)
- Microsoft.AspNet.Web.Optimization (1.1.3)
- WebGrease (1.6.0)
- Antlr (3.5.0.2)
- Microsoft.Web.Infrastructure (2.0.0)

## Project Architecture

### Controllers

- **AccountController**: Handles login/logout operations
  - `Login()` GET/POST - Displays login form and processes credentials
  - `Logout()` - Signs out user and redirects to home
  - Uses claims-based identity with OWIN authentication manager

- **HomeController**: Displays home page

### Models

- **LoginViewModel**: Login form data model
  - Username (required)
  - Password (required)
  - RememberMe (optional)

### Views

- **Login.cshtml**: Bootstrap 5 styled login form with demo credentials display
- **_Layout.cshtml**: Master layout with authentication-aware navbar
- **Index.cshtml**: Home page

## License

This is a scaffolded project template. Add your own license as needed.
