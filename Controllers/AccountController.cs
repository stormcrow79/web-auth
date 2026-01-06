using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using WebAuth.Models;

namespace WebAuth.Controllers
{
    public class AccountController : Controller
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // GET: Account/Login
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Demo: Hardcoded credentials
            if (ValidateCredentials(model.Username, model.Password))
            {
                SignInUser(model.Username, model.RememberMe);

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return View(model);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            // Check if user authenticated via Auth0
            var authProvider = Session["user:provider"]?.ToString();

            // Clear session
            Session.Clear();
            Session.Abandon();

            // Sign out from cookie
            AuthenticationManager.SignOut(Microsoft.Owin.Security.Cookies.CookieAuthenticationDefaults.AuthenticationType);

            // If Auth0 user, also sign out from Auth0
            if (authProvider == "auth0")
            {
                AuthenticationManager.SignOut("Auth0");
            }

            return RedirectToAction("Index", "Home");
        }

        // Login with Auth0
        public ActionResult LoginAuth0(string returnUrl)
        {
            // Redirect to Auth0 login
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = returnUrl ?? Url.Action("Index", "Home")
                },
                "Auth0"
            );
            return new HttpUnauthorizedResult();
        }

        // Auth0 callback handler
        public ActionResult Auth0Callback()
        {
            return RedirectToAction("Index", "Home");
        }

        // Helper method for demo credential validation
        private bool ValidateCredentials(string username, string password)
        {
            // Demo purposes only - in production, use proper password hashing and database
            return username == "demo" && password == "password";
        }

        // Helper method to sign in user
        private void SignInUser(string username, bool rememberMe)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Name, username),
                new Claim("auth_provider", "local")
            };

            var identity = new ClaimsIdentity(claims, Microsoft.Owin.Security.Cookies.CookieAuthenticationDefaults.AuthenticationType);

            // Store in session for local users too
            Session["user:name"] = username;
            Session["user:provider"] = "local";

            AuthenticationManager.SignIn(
                new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                },
                identity
            );
        }
    }
}
