using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace App
{
	/*
	 * Loads and configures OWIN middleware.
	 * This is not the default Implementation 
	 * (Default would be: Start class in the global namespace with a method Configuration, which is calles by the OWIN infrastructure and passed an implementation of the Owin.IAppBuilder.)
	 * Gets called by the <appSettings> in Web.config.
	 */
	public class IdentityConfig
	{
		public void Configuration(IAppBuilder appBuilder)
		{
			appBuilder.CreatePerOwinContext<AppIdentityDbContext>(AppIdentityDbContext.Create);
			appBuilder.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
			appBuilder.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);

			/*
			 * Tells ASP.NET Identity how to use a cookie to identity authendicated users.
			 * The LoginPath property specifies a URL that clients should be redirected to when the request content without authentication. A controller will have to handle these redirections.
			 */
			appBuilder.UseCookieAuthentication(new CookieAuthenticationOptions()
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account/Login"),
			});
		}
	}
}