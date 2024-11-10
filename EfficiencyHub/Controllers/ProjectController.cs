using EfficiencyHub.Common.Enums;
using EfficiencyHub.Data.Models;
using EfficiencyHub.Services.Data;
using EfficiencyHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new ProjectCreateViewModel
            {
                AvailableRoles = Enum.GetValues(typeof(ProjectRole)).Cast<ProjectRole>().ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = Enum.GetValues(typeof(ProjectRole)).Cast<ProjectRole>().ToList();
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var success = await _projectService.CreateProjectAsync(model, user.Id);
            if (!success)
            {
                ModelState.AddModelError("", "Unable to create project. Please try again.");
                model.AvailableRoles = Enum.GetValues(typeof(ProjectRole)).Cast<ProjectRole>().ToList();
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var projects = await _projectService.GetProjectsForUserAsync(user.Id);
            return View(projects);
        }
    }
}
