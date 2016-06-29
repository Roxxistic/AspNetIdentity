using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;
using App.Identity;
using App.Models;

namespace App.ViewModels
{
	public class GlobalViewModel
	{
		public List<string> ErrorMessages { get; set; }
		public List<string> ConfirmationMessages { get; set; }
	}

	public class UserAdminCreateViewModel : GlobalViewModel
	{
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
	}

	public class UserAdminEditViewModel
	{
		[Required]
		public string Id { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
	}

	public class UserAdminEditPasswordViewModel
	{
		[Required]
		public string Id { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		[Compare("Password", ErrorMessage = "The Passwords do not match!")]
		public string ConfirmPassword { get; set; }
	}

	public class LoginViewModel
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string Password { get; set; }
	}

	public class UserAdminEditRoleViewModel
	{
		public string Id { get; set; }
		public string Email { get; set; }
		public List<string> AllRoleNames { get; set; }
		public List<string> AssignedRoleNames { get; set; }
		public string[] SelectedRoleNames { get; set; }
	}

	public class Membership
	{
		public string RoleId { get; set; }
		public string RoleName { get; set; }
		public bool UserIsMemberOfRole { get; set; }
	}

	public class RoleEditModel
	{
		public AppRole Role { get; set; }
		public IEnumerable<AppUser> Members { get; set; }
		public IEnumerable<AppUser> NonMembers { get; set; }
		
	}

	public class RoleModificationModel
	{
		[Required]
		public string RoleName { get; set; }

		public string[] IdsToAdd { get; set; }
		public string[] IdsToDelete { get; set; }
	}

	public class RoleAdminEditUsersViewModel
	{
		public string RoleId { get; set; }
		public string RoleName { get; set; }
		public List<AppUser> AllUsers { get; set; }
		public List<AppUser> AssignedUsers { get; set; }
		public List<string> AllUserNames { get; set; }
		public List<string> AssignedUserNames { get; set; }
		public string[] SelectedUserNames { get; set; }
	}
}