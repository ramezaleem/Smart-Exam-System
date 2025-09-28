using ExamSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserManagementController ( UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index ()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public IActionResult Create ()
        {
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create ( ApplicationUser user, string password, string role )
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(user);
        }

        public async Task<IActionResult> Edit ( string id )
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            ViewBag.UserRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit ( string id, ApplicationUser user, string role )
        {
            var oldUser = await _userManager.FindByIdAsync(id);
            if (oldUser == null)
                return NotFound();
            oldUser.FullName = user.FullName;
            oldUser.Email = user.Email;
            oldUser.UserName = user.UserName;
            var result = await _userManager.UpdateAsync(oldUser);
            if (result.Succeeded)
            {
                var oldRoles = await _userManager.GetRolesAsync(oldUser);
                await _userManager.RemoveFromRolesAsync(oldUser, oldRoles);
                await _userManager.AddToRoleAsync(oldUser, role);
                return RedirectToAction("Index");
            }
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            ViewBag.UserRole = role;
            return View(user);
        }

        public async Task<IActionResult> Delete ( string id )
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed ( string id )
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
                await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
    }
}
