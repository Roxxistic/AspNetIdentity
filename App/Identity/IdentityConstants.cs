using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Identity
{
	public static class IdentityConstants
	{
		// Roles
		public static string AdminRole = "SuperAdmin";
		public static string MinorRole = "Editor";

		// Default User
		public static string UserEmail = "superadmin@example.com";
		public static string UserName = UserEmail;
		public static string UserPassword = "W3$tSde";
	}
}