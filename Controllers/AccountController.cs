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
            AuthenticationManager.SignOut("ApplicationCookie");
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
                new Claim(ClaimTypes.Name, username)
            };

            var identity = new ClaimsIdentity(claims, "ApplicationCookie");
            var principal = new ClaimsPrincipal(identity);

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
