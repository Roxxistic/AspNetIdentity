using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using App.Identity;
using App.Models;
using App.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace App.Controllers
{
    [Authorize]
	public class RoleAdminController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
            return View(RoleManager.Roles);
        }

		// GET: Create
	    public ActionResult Create()
	    {
		    return View();
	    }

		// POST: Create
	    [HttpPost]
	    public async Task<ActionResult> Create([Required] string name)
	    {
		    if (ModelState.IsValid)
		    {
			    IdentityResult result = await RoleManager.CreateAsync(new AppRole(name));
			    if (result.Succeeded)
			    {
				    return RedirectToAction("Index");
			    }
			    else
			    {
				    AddErrorsFromResult(result);
			    }
		    }
		    return View(name);
	    }


		// GET: Edit Users
		public async Task<ActionResult> EditUsers(string id)
		{
			AppRole role = await RoleManager.FindByIdAsync(id);
			var currentUserName = User.Identity.GetUserName();

			if (role == null)
			{
				return RedirectToAction("Index");
			}

			var viewmodel = new RoleAdminEditUsersViewModel()
			{
				RoleId = role.Id,
				RoleName = role.Name,
				AllUserNames = UserManager.Users.Select(u => u.UserName).ToList(),
				AssignedUserNames = UserManager.Users.Where(u => u.Roles.Any(r => r.RoleId == role.Id)).Select(u => u.UserName).ToList(),
				AllUsers = UserManager.Users.ToList(),
				AssignedUsers = UserManager.Users.Where(u => u.Roles.Any(r => r.RoleId == role.Id)).ToList()
			};
			return View(viewmodel);
		}

		// POST: Edit Users
		[HttpPost]
		public async Task<ActionResult> EditUsers(RoleAdminEditUsersViewModel viewmodel, string[] checkboxSelectedUsers)
		{
			var roleId = viewmodel.RoleId;
			var roleName = viewmodel.RoleName;
			var selectedUserIds = new string[]{""};
			selectedUserIds = checkboxSelectedUsers;
			var assignedUserIds = UserManager.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId)).Select(u => u.Id).ToList();
			var allUserIds = UserManager.Users.Select(u => u.Id).ToList();

			var currentPrincipalId = User.Identity.GetUserId();


			foreach(var userId in allUserIds )
			{
				IdentityResult result;
				// Add Role Assignment to User
				if (selectedUserIds.Contains(userId) && !assignedUserIds.Contains(userId))
				{
					if (userId != currentPrincipalId)
					{
						result = await UserManager.AddToRoleAsync(userId, roleName);
						if (!result.Succeeded)
						{
							return RedirectToAction("Index");
						}
					}
				}
				// Remove Role Assignment from User
				if (!selectedUserIds.Contains(userId) && assignedUserIds.Contains(userId))
				{
					if (userId != currentPrincipalId)
					{
						result = await UserManager.RemoveFromRoleAsync(userId, roleName);
						if (!result.Succeeded)
						{
							return RedirectToAction("Index");
						}
					}
				}
			}

			return View("Index");
		}


		// GET: Edit
	    public async Task<ActionResult> Edit(string id)
	    {
		    AppRole role = await RoleManager.FindByIdAsync(id);
		    string[] memberIds = role.Users.Select(x => x.UserId).ToArray();
		    IEnumerable<AppUser> members = UserManager.Users.Where(x => memberIds.Any(y => y == x.Id));
		    IEnumerable<AppUser> nonMembers = UserManager.Users.Except(members);
			RoleEditModel viewmodel = new RoleEditModel(){ Role = role, Members = members, NonMembers = nonMembers};
		    return View(viewmodel);
	    }

		// POST: Edit
	    [HttpPost]
	    public async Task<ActionResult> Edit(RoleModificationModel model)
	    {
		    IdentityResult result;
		    if (ModelState.IsValid)
		    {
			    foreach (string userId in model.IdsToAdd ?? new string[] {})
			    {
				    result = await UserManager.AddToRoleAsync(userId, model.RoleName);
				    if (!result.Succeeded)
				    {
						return RedirectToAction("Index");
				    }
			    }
			    foreach (string userId in model.IdsToDelete ?? new string[] {})
			    {
				    result = await UserManager.RemoveFromRoleAsync(userId, model.RoleName);
				    if (!result.Succeeded)
				    {
						return RedirectToAction("Index");
				    }
			    }
			    return RedirectToAction("Index");
		    }
			return RedirectToAction("Index");
	    }


	    private void AddErrorsFromResult(IdentityResult result)
	    {
		    foreach (string error in result.Errors)
		    {
			    ModelState.AddModelError("", error);
		    }
	    }

	    private AppUserManager UserManager
	    {
		    get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
	    }

	    private AppRoleManager RoleManager
	    {
		    get { return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>(); }
	    }
    }
}