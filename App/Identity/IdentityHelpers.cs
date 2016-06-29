using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;

namespace App.Identity
{
	public static class IdentityHelpers
	{
		public static MvcHtmlString GetUserName(this HtmlHelper html, string id)
		{
			AppUserManager manager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
			return new MvcHtmlString(manager.FindByIdAsync(id).Result.UserName);
		}

		public static MvcHtmlString GetRoleNameByRoleId(this HtmlHelper html, string id)
		{
			AppRoleManager manager = HttpContext.Current.GetOwinContext().GetUserManager<AppRoleManager>();
			return new MvcHtmlString(manager.FindByIdAsync(id).Result.Name);
		}
	}
}