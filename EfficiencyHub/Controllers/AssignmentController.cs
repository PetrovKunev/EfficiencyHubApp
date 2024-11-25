using EfficiencyHub.Data.Models;
using EfficiencyHub.Services.Data;
using EfficiencyHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace EfficiencyHub.Web.Controllers
{
    [Authorize]
    public class AssignmentController : BaseController
    {
        private readonly AssignmentService _assignmentService;
        private readonly ProjectService _projectService;


        public AssignmentController(AssignmentService assignmentService, ProjectService projectService, ILogger<BaseController> logger, UserManager<ApplicationUser> userManager)
            : base(logger, userManager)
        {
            _assignmentService = assignmentService;
            _projectService = projectService;
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
                var projectName = await _assignmentService.GetProjectNameAsync(projectId);

                ViewBag.ProjectId = projectId;
                ViewBag.ProjectName = projectName;

                
                return View(assignments);
            }
            catch (Exception ex)
            {
                LogError("Error loading assignments for project.", ex);
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Create(Guid projectId)
        {
            var projectName = await _projectService.GetProjectNameAsync(projectId);

            var viewModel = new AssignmentCreateViewModel
            {
                ProjectId = projectId,
                DueDate = DateTime.Now.AddDays(7)
            };

            ViewBag.ProjectId = projectId;
            ViewBag.ProjectName = projectName;

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

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
               
                var success = await _assignmentService.CreateAssignmentAsync(model, projectId, user.Id);
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

        [HttpGet]
        public async Task<IActionResult> Edit(Guid projectId, Guid id)
        {
            var assignment = await _assignmentService.GetAssignmentByIdAsync(projectId, id);

            if (assignment == null)
            {
                return NotFound();
            }

            var viewModel = new AssignmentEditViewModel
            {
                Id = assignment.Id,
                Title = assignment.Title,
                Description = assignment.Description,
                DueDate = assignment.DueDate,
                CreatedDate = assignment.CreatedDate,
                Status = assignment.Status,
                ProjectId = projectId
            };

            ViewBag.ProjectId = projectId;
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AssignmentEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid for Edit Assignment: {Model}", model);
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("User is not authenticated.");
                return Unauthorized();
            }

            var success = await _assignmentService.UpdateAssignmentAsync(model, user.Id);

            if (success)
            {
                _logger.LogInformation("Assignment updated successfully. Redirecting to Index for ProjectId: {ProjectId}", model.ProjectId);
                return RedirectToAction(nameof(Index), new { projectId = model.ProjectId });
            }

            _logger.LogError("Failed to update the assignment for ProjectId: {ProjectId}", model.ProjectId);
            ModelState.AddModelError(string.Empty, "Failed to update the assignment. Please try again.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid projectId, Guid assignmentId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var success = await _assignmentService.DeleteAssignmentAsync(projectId, assignmentId, currentUser.Id);

            if (success)
            {
                return RedirectToAction(nameof(Index), new { projectId });
            }

            ModelState.AddModelError(string.Empty, "Failed to delete the assignment.");
            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid projectId, Guid id)
        {
            AssignmentViewModel? assignment = await _assignmentService.GetAssignmentDetailsByIdAsync(projectId, id);

            if (assignment == null)
            {
                return NotFound();
            }

            ViewBag.ProjectId = projectId;
            return View(assignment);
        }
    }
}
