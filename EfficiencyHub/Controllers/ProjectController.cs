using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Services.Data;
using Microsoft.AspNetCore.Identity;

namespace EfficiencyHub.Web.Controllers
{
    [Authorize]
    public class ProjectController : BaseController
    {
        private readonly ProjectService _projectService;

        public ProjectController(ProjectService projectService, ILogger<BaseController> logger, UserManager<ApplicationUser> userManager)
            : base(logger, userManager)
        {
            _projectService = projectService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                LogError("Unable to retrieve the current user.", new Exception("User not found"));
                return RedirectToAction("Login", "Account");
            }

            var projects = await _projectService.GetProjectsForUserAsync(currentUser.Id);
            return View(projects);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();
                if (currentUser == null)
                {
                    LogError("Unable to retrieve the current user for creating a project.", new Exception("User not found"));
                    return RedirectToAction("Login", "Account");
                }

                var success = await _projectService.CreateProjectAsync(project, currentUser.Id);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                LogError("Failed to create project", new Exception("Project creation failed"));
            }
            return View(project);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                LogError("Project not found for editing", new Exception($"Project with ID {id} not found"));
                return NotFound();
            }
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var success = await _projectService.UpdateProjectAsync(project);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                LogError("Failed to update project", new Exception("Project update failed"));
            }
            return View(project);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                LogError("Project not found for deletion", new Exception($"Project with ID {id} not found"));
                return NotFound();
            }
            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var success = await _projectService.DeleteProjectAsync(id);
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            LogError("Failed to delete project", new Exception("Project deletion failed"));
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
