using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof(WebAuth.Startup))]

namespace WebAuth
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            ConfigureAuth(app);
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType("Cookies");

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
                PostLogoutRedirectUri = ConfigurationManager.AppSettings["auth0:PostLogoutRedirectUri"],

                ResponseType = OpenIdConnectResponseType.CodeIdToken,
                Scope = "openid profile email",

                SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType,

                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                },

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    // Store ID token claims in session
                    SecurityTokenValidated = notification =>
                    {
                        var identity = notification.AuthenticationTicket.Identity;

                        // Add custom claim to identify Auth0 users
                        identity.AddClaim(new Claim("auth_provider", "auth0"));

                        // Store claims in session
                        var session = HttpContext.Current.Session;
                        session["user:name"] = identity.FindFirst("name")?.Value;
                        session["user:email"] = identity.FindFirst("email")?.Value;
                        session["user:picture"] = identity.FindFirst("picture")?.Value;
                        session["user:sub"] = identity.FindFirst("sub")?.Value;
                        session["user:provider"] = "auth0";

                        return Task.FromResult(0);
                    },

                    RedirectToIdentityProvider = notification =>
                    {
                        // Handle logout
                        if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var logoutUri = $"https://{ConfigurationManager.AppSettings["auth0:Domain"]}/v2/logout?client_id={ConfigurationManager.AppSettings["auth0:ClientId"]}";
                            var postLogoutUri = notification.ProtocolMessage.PostLogoutRedirectUri;
                            if (!string.IsNullOrEmpty(postLogoutUri))
                            {
                                logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                            }
                            notification.Response.Redirect(logoutUri);
                            notification.HandleResponse();
                        }
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}
