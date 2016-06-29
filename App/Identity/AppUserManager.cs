using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using App.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace App.Identity
{
	public class AppUserManager : UserManager<AppUser>
	{
		public AppUserManager(IUserStore<AppUser> store) : base(store) { }

		/*
		 * Called when Identity needs an instance of the AppUserManager, which will happen when operations on user data are performed.
		 * 
		 * To create an instance of the AppUserManager, an instance of UserStore<AppUser> is needed. 
		 * The UserStore<T> is the EF implementation of the IUserStore<T>, which provides storage-specific implementation of the methods definde by the UserManager class.
		 * 
		 * To create the UserStore<AppUser>, an instance of the AppIdentityDbContext class is needed. This is provided by OWIN. 
		 * The IOwinContext implementation passed as an arguemtn to the Create method defines a generically typed Get method that returns instances of objects that have been registered in the OWIN start class.
		 */
		public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
		{
			AppIdentityDbContext db = context.Get<AppIdentityDbContext>();
			AppUserManager manager = new AppUserManager(new UserStore<AppUser>(db));

			/*
			 * Implements Password Policy
			 */
			manager.PasswordValidator = new PasswordValidator()
			{
				RequiredLength = 6,
				RequireNonLetterOrDigit = false,
				RequireDigit = false,
				RequireLowercase = true,
				RequireUppercase = true
			};

			/*
			 * Implements UserName Policy
			 */
			manager.UserValidator = new UserValidator<AppUser>(manager) // You would implement the CustomUserValidator here. Currently base used.
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};

			return manager;
		}
	}

	public class CustomUserValidator : UserValidator<AppUser>
	{
		public CustomUserValidator(AppUserManager manager) : base(manager) { }
		public override async Task<IdentityResult> ValidateAsync(AppUser user)
		{
			IdentityResult result = await base.ValidateAsync(user);
			// custom Implementation would go here
			return result;
		}
	}
}