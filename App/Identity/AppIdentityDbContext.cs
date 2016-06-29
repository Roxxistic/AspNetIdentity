using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using App.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace App.Identity
{
	public class AppIdentityDbContext : IdentityDbContext<AppUser>
	{
		/* 
		 * Associates the connection string defined in Web.config with ASP.NET Identity. Therefor the constructor calls its base with the name of the connection string.
		 */
		public AppIdentityDbContext() : base("IdentityAppDb2") { }

		/* 
		 * Specifies a class that will seed the database when the shema is first created through EF CF.
		 */
		static AppIdentityDbContext()
		{
			Database.SetInitializer<AppIdentityDbContext>(new IdentityDbInit());
		}

		/* 
		 * This is how instances of the class will be created when needed by the OWIN, using the Start Class "App.IdentityConfig" which is referenced in the Web.config.
		 */
		public static AppIdentityDbContext Create()
		{
			return new AppIdentityDbContext();
		}
	}

	/*
	 * The Seed Class.
	 */
	public class IdentityDbInit : DropCreateDatabaseIfModelChanges<AppIdentityDbContext>
	{
		protected override void Seed(AppIdentityDbContext context)
		{
			PerformInitialSetup(context);
			base.Seed(context);
		}

		/* 
		 * Used to perform the Seeding.
		 */
		public void PerformInitialSetup(AppIdentityDbContext context)
		{
			// Seed the Default Roles

			var mandatoryAdminRole = IdentityConstants.AdminRole;
			var mandatoryMinorRole = IdentityConstants.MinorRole;

			if (!context.Roles.Any(r => r.Name == mandatoryAdminRole))
			{
				var roleStore = new RoleStore<AppRole>(context);
				var roleManager = new AppRoleManager(roleStore);
				var role = new AppRole() { Name = mandatoryAdminRole };
				roleManager.Create(role);
			}

			if (!context.Roles.Any(r => r.Name == mandatoryMinorRole))
			{
				var roleStore = new RoleStore<AppRole>(context);
				var roleManager = new AppRoleManager(roleStore);
				var role = new AppRole() { Name = mandatoryMinorRole };
				roleManager.Create(role);
			}
			
			// Seed the Default Admin

			var userEmail = IdentityConstants.UserEmail;
			var userName = userEmail;
			var userPassword = IdentityConstants.UserPassword;
			
			if (!context.Users.Any(u => u.UserName == userName))
			{
				var userStore = new UserStore<AppUser>(context);
				var userManager = new AppUserManager(userStore);
				var user = new AppUser()
				{
					UserName = userName,
					Email = userEmail,
				};
				userManager.Create(user, userPassword);
				userManager.AddToRole(user.Id, mandatoryAdminRole);
			}
			
		}
	}
}