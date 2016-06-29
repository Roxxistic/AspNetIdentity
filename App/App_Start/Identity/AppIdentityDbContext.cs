using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using App.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace App.Identity
{
	public class AppIdentityDbContext : IdentityDbContext<AppUser>
	{
		public AppIdentityDbContext() : base("IdentityDb") { }

		static AppIdentityDbContext()
		{
			Database.SetInitializer<AppIdentityDbContext>(new IdentityDbInit());
		}

		public static AppIdentityDbContext Create()
		{
			return new AppIdentityDbContext();
		}
	}

	public class IdentityDbInit : DropCreateDatabaseIfModelChanges<AppIdentityDbContext>
	{
		protected override void Seed(AppIdentityDbContext context)
		{
			PerformInitialSetup(context);
			base.Seed(context);
		}

		public void PerformInitialSetup(AppIdentityDbContext context)
		{
			// Initial Configuration will go here.
		}
	}
}