# WebAuth - ASP.NET MVC Application with OWIN

A scaffolded ASP.NET MVC application using .NET Framework 4.8 and OWIN middleware.

## Project Structure

```
WebAuth/
├── App_Start/
│   └── RouteConfig.cs          # MVC route configuration
├── Content/
│   └── Site.css                # Application styles
├── Controllers/
│   └── HomeController.cs       # Home controller with Index, About, Contact actions
├── Models/                     # Data models (empty)
├── Properties/
│   └── AssemblyInfo.cs         # Assembly metadata
├── Scripts/                    # JavaScript files
├── Views/
│   ├── Home/
│   │   └── Index.cshtml        # Home page view
│   ├── Shared/
│   │   └── _Layout.cshtml      # Shared layout template
│   ├── _ViewStart.cshtml       # View initialization
│   └── Web.config              # Views configuration
├── Global.asax                 # Application events
├── Global.asax.cs              # Application startup
├── Startup.cs                  # OWIN startup configuration
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

- **ASP.NET MVC 5**: Model-View-Controller architecture
- **OWIN Middleware**: Configured in Startup.cs for authentication and other middleware
- **.NET Framework 4.8**: Latest version of the .NET Framework
- **Razor Views**: Dynamic page rendering with Razor syntax

## OWIN Configuration

The OWIN startup class is located in `Startup.cs`. You can add authentication middleware here:

```csharp
public void Configuration(IAppBuilder app)
{
    ConfigureAuth(app);
}

private void ConfigureAuth(IAppBuilder app)
{
    // Add authentication middleware here
    // Example: Cookie authentication, OAuth, etc.
}
```

## NuGet Packages Included

- Microsoft.AspNet.Mvc (5.3.0)
- Microsoft.AspNet.Razor (3.3.0)
- Microsoft.AspNet.WebPages (3.3.0)
- Microsoft.Owin (4.2.3)
- Microsoft.Owin.Host.SystemWeb (4.2.3)
- Microsoft.CodeDom.Providers.DotNetCompilerPlatform (4.1.0)
- Owin (1.0)
- Newtonsoft.Json (13.0.4)
- Microsoft.AspNet.Web.Optimization (1.1.3)
- WebGrease (1.6.0)
- Antlr (3.5.0.2)
- Microsoft.Web.Infrastructure (2.0.0)

## Next Steps

To add authentication:
1. Install authentication packages (e.g., `Microsoft.Owin.Security.Cookies`)
2. Configure authentication in `Startup.cs`
3. Add authentication attributes to controllers
4. Create login/logout views and actions

## License

This is a scaffolded project template. Add your own license as needed.
