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

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
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

        
        public IActionResult ManageData()
        {
            var projects = _context.Projects.ToList();
            var tasks = _context.Tasks.ToList();
            var reminders = _context.Reminders.ToList();

            return View(new AdminDataViewModel
            {
                Projects = projects,
                Tasks = tasks,
                Reminders = reminders
            });
        }

        [HttpPost]
        public IActionResult DeleteProject(Guid projectId)
        {
            var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project == null) return NotFound();

            _context.Projects.Remove(project);
            _context.SaveChanges();

            return RedirectToAction(nameof(ManageData));
        }

        [HttpPost]
        public IActionResult DeleteTask(Guid taskId)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return NotFound();

            _context.Tasks.Remove(task);
            _context.SaveChanges();

            return RedirectToAction(nameof(ManageData));
        }

        [HttpPost]
        public IActionResult DeleteReminder(Guid reminderId)
        {
            var reminder = _context.Reminders.FirstOrDefault(r => r.Id == reminderId);
            if (reminder == null) return NotFound();

            _context.Reminders.Remove(reminder);
            _context.SaveChanges();

            return RedirectToAction(nameof(ManageData));
        }

        [HttpGet]
        public IActionResult EditProject(Guid projectId)
        {
            var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project == null) return NotFound();

            return View(project);
        }

        [HttpPost]
        public IActionResult EditProject(Project project)
        {
            if (!ModelState.IsValid) return View(project);

            _context.Projects.Update(project);
            _context.SaveChanges();

            return RedirectToAction(nameof(ManageData));
        }

        [HttpGet]
        public IActionResult EditTask(Guid taskId)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return NotFound();

            return View(task);
        }

        [HttpPost]
        public IActionResult EditTask(Assignment task)
        {
            if (!ModelState.IsValid) return View(task);

            _context.Tasks.Update(task);
            _context.SaveChanges();

            return RedirectToAction(nameof(ManageData));
        }

        [HttpGet]
        public IActionResult EditReminder(Guid reminderId)
        {
            var reminder = _context.Reminders.FirstOrDefault(r => r.Id == reminderId);
            if (reminder == null) return NotFound();

            return View(reminder);
        }

        [HttpPost]
        public IActionResult EditReminder(Reminder reminder)
        {
            if (!ModelState.IsValid) return View(reminder);

            _context.Reminders.Update(reminder);
            _context.SaveChanges();

            return RedirectToAction(nameof(ManageData));
        }

    }
}
