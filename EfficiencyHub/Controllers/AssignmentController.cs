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


        //[HttpGet]
        //public IActionResult Create(Guid projectId)
        //{
        //    var viewModel = new AssignmentCreateViewModel
        //    {
        //        ProjectId = projectId,
        //        DueDate = DateTime.Now.AddDays(7)
        //    };
        //    ViewBag.ProjectId = projectId;
        //    return View(viewModel);
        //}

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

            var user = await _userManager.GetUserAsync(User); // Вземаме текущия потребител
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                // Предаваме UserId на текущия потребител към сервиза
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
                return View(model);
            }

            var success = await _assignmentService.UpdateAssignmentAsync(model);

            if (success)
            {
                // Redirect to the Index action, passing the projectId to load tasks for that project
                return RedirectToAction(nameof(Index), new { projectId = model.ProjectId });
            }

            ModelState.AddModelError(string.Empty, "Failed to update the assignment. Please try again.");
            return View(model);
        }


       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid projectId, Guid assignmentId)
        {
            var result = await _assignmentService.DeleteAssignmentAsync(projectId, assignmentId);

            if (!result)
            {
                TempData["Error"] = "Failed to delete the assignment.";
            }

            return RedirectToAction("Index", new { projectId = projectId });
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
