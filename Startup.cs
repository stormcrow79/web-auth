using System;
using Microsoft.Owin;
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
            // Configure authentication middleware here
            // Example: app.UseCookieAuthentication(new CookieAuthenticationOptions());
        }
    }
}
