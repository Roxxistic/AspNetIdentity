using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using App.Identity;
using App.Models;
using App.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace App.Controllers
{
	[Authorize]
    public class UserAdminController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
			return View(UserManager.Users);
        }

		// GET: Create
	    public ActionResult Create()
	    {
		    return View();
	    }

		// POST: Create
	    [HttpPost]
	    public async Task<ActionResult> Create(UserAdminCreateViewModel model)
	    {
		    if (ModelState.IsValid)
		    {
			    AppUser user = new AppUser()
			    {
				    UserName = model.Email,
					Email = model.Email
			    };
			    IdentityResult addUserResult = await UserManager.CreateAsync(user, model.Password);
				if (!addUserResult.Succeeded)
			    {
					AddErrorsFromResult(addUserResult);
					View(model);
			    }
				IdentityResult addRoleToUserResult = await UserManager.AddToRoleAsync(user.Id, IdentityConstants.MinorRole);
				if (!addRoleToUserResult.Succeeded)
				{
					AddErrorsFromResult(addRoleToUserResult);
					View(model);
				}
		    }
			return RedirectToAction("Index");
	    }

		// GET: Edit
	    public async Task<ActionResult> Edit(string id)
	    {
		    AppUser user = await UserManager.FindByIdAsync(id);
		    if (user != null)
		    {
			    return View(user);
		    }
		    else
		    {
			    return RedirectToAction("Index");
		    }
	    }

		// POST: Edit
	    [HttpPost]
	    public async Task<ActionResult> Edit(UserAdminEditViewModel viewmodel)
	    {
		    string id = viewmodel.Id;
		    string email = viewmodel.Email;
		    string password = viewmodel.Password;
		    string username = viewmodel.Email;
			
			AppUser user = await UserManager.FindByIdAsync(id);
		    if (user != null)
		    {
			    user.Email = email;
			    user.UserName = username;
			    IdentityResult validEmail = await UserManager.UserValidator.ValidateAsync(user);
			    if (!validEmail.Succeeded)
			    {
				    AddErrorsFromResult(validEmail);
			    }
			    IdentityResult validPass = null;
			    if (password != string.Empty)
			    {
				    validPass = await UserManager.PasswordValidator.ValidateAsync(password);
				    if (validPass.Succeeded)
				    {
					    user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
				    }
				    else
				    {
					    AddErrorsFromResult(validPass);
				    }
			    }
			    if ((validEmail.Succeeded && validPass == null) ||
			        (validEmail.Succeeded && password != string.Empty && validPass.Succeeded))
			    {
				    IdentityResult result = await UserManager.UpdateAsync(user);
				    if (result.Succeeded)
				    {
					    return RedirectToAction("Index");
				    }
				    else
				    {
					    AddErrorsFromResult(result);
				    }
			    }
		    }
		    else
		    {
			    ModelState.AddModelError("", "User Not Found");
		    }
		    return View(user);
	    }

		// GET: Edit Password
		public async Task<ActionResult> EditPassword(string id)
		{
			AppUser user = await UserManager.FindByIdAsync(id);
			if (user == null)
			{
				return RedirectToAction("Index");
			}
			var viewmodel = new UserAdminEditPasswordViewModel()
			{
				Id = user.Id,
				Email = user.Email,
			};
			return View(viewmodel);
		}

		// POST: Edit Password
		[HttpPost]
		public async Task<ActionResult> EditPassword(UserAdminEditPasswordViewModel input)
		{
			string id = input.Id;
			string password = input.Password;
			string confirmPassword = input.ConfirmPassword;

			AppUser user = await UserManager.FindByIdAsync(id);

			if (user == null)
			{
				ModelState.AddModelError("", "User Not Found");
				return RedirectToAction("Index");
			}

			UserAdminEditPasswordViewModel viewmodel = new UserAdminEditPasswordViewModel()
			{
				Id = user.Id,
				Email = user.Email
			};

			if (input.Password == null)
			{
				ModelState.AddModelError("", "You must either enter a new Password or cancel!");
				return View(viewmodel);
			}

			if (password != confirmPassword)
			{
				ModelState.AddModelError("", "Password and Confirm Password do not match!");
				return View(viewmodel);
			}

			IdentityResult validPass = await UserManager.PasswordValidator.ValidateAsync(password);
			if (validPass.Succeeded)
			{
				user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
			}
			else
			{
				AddErrorsFromResult(validPass);
				return View(input);

			}

			IdentityResult result = await UserManager.UpdateAsync(user);
			if (result.Succeeded)
			{
				return RedirectToAction("Index");
			}
			else
			{
				AddErrorsFromResult(result);
				return View(input);
			}
		}
		
		// GET: Edit Role
	    public async Task<ActionResult> EditRole(string id)
	    {
			AppUser user = await UserManager.FindByIdAsync(id);
		    var currentUserId = User.Identity.GetUserId(); 
			if (user == null || user.Id == currentUserId)
			{
				return RedirectToAction("Index");
			}
		    var viewmodel = new UserAdminEditRoleViewModel
		    {
			    Id = user.Id,
			    Email = user.Email,
			    AllRoleNames = RoleManager.Roles.Select(r => r.Name).ToList(),
			    AssignedRoleNames =
				    RoleManager.Roles.Where(r => r.Users.Any(u => u.UserId == user.Id)).Select(r => r.Name).ToList()
		    };
		    return View(viewmodel);
	    }

		// POST: Edit Role
	    [HttpPost]
		public async Task<ActionResult> EditRole(UserAdminEditRoleViewModel viewmodel, string[] checkboxSelectedRoleNames)
	    {
		    // Prevent SuperAdmin to remove his own Role
			var currentUserIsSuperAdmin = User.IsInRole("SuperAdmin");
		    if (currentUserIsSuperAdmin)
		    {
				return RedirectToAction("Index");
		    }

			var userId = viewmodel.Id;
			AppUser user = await UserManager.FindByIdAsync(userId);

			var allRoleNames = RoleManager.Roles.Select(r => r.Name).ToList();
			var assignedRoleNames = RoleManager.Roles.Where(r => r.Users.Any(u => u.UserId == user.Id)).Select(r => r.Name).ToList();
		    var selectedRoleNames = checkboxSelectedRoleNames;

		    
		    foreach (var roleName in allRoleNames)
		    {
				IdentityResult result;
				// Add Role Assignment to User
			    if (selectedRoleNames.Contains(roleName) && !assignedRoleNames.Contains(roleName))
			    {
				    result = await UserManager.AddToRoleAsync(userId, roleName);
					if (!result.Succeeded)
					{
						return RedirectToAction("Index");
					}
			    }

				// Remove Role Assignment from User
				if (!selectedRoleNames.Contains(roleName) && assignedRoleNames.Contains(roleName))
				{
					result = await UserManager.RemoveFromRoleAsync(userId, roleName);
					if (!result.Succeeded)
					{
						return RedirectToAction("Index");
					}
				}
		    }
			return RedirectToAction("Index");
	    }

		private AppUserManager UserManager
	    {
		    get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
	    }

		private AppRoleManager RoleManager
		{
			get { return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>(); }
		}

	    private void AddErrorsFromResult(IdentityResult result)
	    {
		    foreach (string error in result.Errors)
		    {
			    ModelState.AddModelError("", error);
		    }
	    }
    }
}