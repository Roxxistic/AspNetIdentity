using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using App.Identity;
using App.Models;
using App.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace App.Controllers
{
    [Authorize]
	public class AccountController : Controller
    {
        // GET: Login
        [AllowAnonymous]
		public ActionResult Login(string returnUrl)
        {
	        ViewBag.returnUrl = returnUrl;
			return View();
        }

		// POST: Login
	    [HttpPost]
	    [AllowAnonymous]
	    [ValidateAntiForgeryToken]
	    public async Task<ActionResult> Login(LoginViewModel viewmodel, string returnUrl)
	    {
			if (ModelState.IsValid)
			{
				AppUser user = await UserManager.FindAsync(viewmodel.Name, viewmodel.Password);

				if (user == null)
				{
					ModelState.AddModelError("", "Invalid Username or Password");
				}
				else
				{
					ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
					AuthManager.SignOut();
					AuthManager.SignIn(new AuthenticationProperties(){IsPersistent = false}, ident);

					if (string.IsNullOrEmpty(returnUrl))
					{
						return RedirectToAction("Index", "Home");
					}
					else
					{
						return Redirect(returnUrl);
					}
				}
			}
			return View(viewmodel);
	    }

		// Logout

	    public ActionResult Logout()
		{
			AuthManager.SignOut();
			return RedirectToAction("Index", "Home");
		}

	    private IAuthenticationManager AuthManager
	    {
		    get { return HttpContext.GetOwinContext().Authentication; }
	    }

	    private AppUserManager UserManager
	    {
		    get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
	    }
    }
}