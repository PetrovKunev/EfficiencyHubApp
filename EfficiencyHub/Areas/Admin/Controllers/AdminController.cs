using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Data;
using EfficiencyHub.Services.Data;
using EfficiencyHub.Web.Areas.Admin.Models;



namespace EfficiencyHub.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ProjectService _projectService;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, ApplicationDbContext context, ProjectService projectService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _projectService = projectService;
        }



        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserWithRolesViewModel
                {
                    User = user,
                    Roles = string.Join(", ", roles)
                });
            }

            return View(userRoles);
        }

        [HttpPost]
        public async Task<IActionResult> MakeAdmin(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            if (!await _roleManager.RoleExistsAsync("Administrator"))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = "Administrator" });
            }

            var result = await _userManager.AddToRoleAsync(user, "Administrator");

            if (result.Succeeded)
            {
                TempData["Success"] = $"User {user.UserName} is now an Administrator.";
            }
            else
            {
                TempData["Error"] = $"Failed to make user an Administrator: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        
        public async Task<IActionResult> RemoveAdmin(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Administrator"))
            {
                var result = await _userManager.RemoveFromRoleAsync(user, "Administrator");

                if (result.Succeeded)
                {
                    TempData["Success"] = $"User {user.UserName} is no longer an Administrator.";
                }
                else
                {
                    TempData["Error"] = $"Failed to remove Administrator role: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                }
            }

            return RedirectToAction(nameof(ManageUsers));
        }


    }

}
