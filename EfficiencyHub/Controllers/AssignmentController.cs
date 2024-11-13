using EfficiencyHub.Data.Models;
using EfficiencyHub.Services.Data;
using EfficiencyHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EfficiencyHub.Web.Controllers
{
    [Authorize]
    public class AssignmentController : BaseController
    {
        private readonly AssignmentService _assignmentService;

        public AssignmentController(AssignmentService assignmentService, ILogger<BaseController> logger, UserManager<ApplicationUser> userManager)
            : base(logger, userManager)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid projectId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                var assignments = await _assignmentService.GetAssignmentsForProjectAsync(projectId);
                ViewBag.ProjectId = projectId;
                return View(assignments);
            }
            catch (Exception ex)
            {
                LogError("Error loading assignments for project.", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Create(Guid projectId)
        {
            var viewModel = new AssignmentCreateViewModel
            {
                DueDate = DateTime.Now.AddDays(7) // Default due date, for example
            };
            ViewBag.ProjectId = projectId;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssignmentCreateViewModel model, Guid projectId)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProjectId = projectId;
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                var success = await _assignmentService.CreateAssignmentAsync(model, projectId);
                if (!success)
                {
                    ModelState.AddModelError("", "Unable to create assignment. Please try again.");
                    ViewBag.ProjectId = projectId;
                    return View(model);
                }

                return RedirectToAction("Index", new { projectId });
            }
            catch (Exception ex)
            {
                LogError("Error creating assignment.", ex);
                ModelState.AddModelError("", "Unable to create assignment. Please try again.");
                ViewBag.ProjectId = projectId;
                return View(model);
            }
        }
    }
}
