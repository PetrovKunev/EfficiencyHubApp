using EfficiencyHub.Data.Models;
using EfficiencyHub.Services.Data;
using EfficiencyHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EfficiencyHub.Web.Controllers
{
    [Authorize]
    public class ReminderController : BaseController
    {
        private readonly ReminderService _reminderService;
        private readonly AssignmentService _assignmentService;

        public ReminderController(ReminderService reminderService,AssignmentService assignmentService , ILogger<BaseController> logger, UserManager<ApplicationUser> userManager)
            : base(logger, userManager)
        {
            _reminderService = reminderService;
            _assignmentService = assignmentService;
        
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid assignmentId)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                var reminders = await _reminderService.GetRemindersByAssignmentAsync(assignmentId, currentUser.Id);
                var assignmentName = await _assignmentService.GetAssignmentNameAsync(assignmentId);
                var projectId = await _assignmentService.GetProjectIdByAssignmentAsync(assignmentId);

                ViewBag.AssignmentId = assignmentId;
                ViewBag.AssignmentName = assignmentName;
                ViewBag.ProjectId = projectId;

                return View(reminders);
            }
            catch (Exception ex)
            {
                LogError("Error loading reminders.", ex);
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpGet]
        public IActionResult Create(Guid assignmentId)
        {
            var viewModel = new ReminderCreateViewModel
            {
                AssignmentId = assignmentId,
                ReminderDate = DateTime.Now.AddDays(1)
            };
            ViewBag.AssignmentId = assignmentId;
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReminderCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    return Unauthorized();
                }

                await _reminderService.AddReminderAsync(model, user.Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogError("Failed to create reminder.", ex);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var reminder = await _reminderService.GetReminderByIdAsync(id);
                if (reminder == null)
                {
                    return NotFound();
                }

                return View(reminder);
            }
            catch (Exception ex)
            {
                LogError("Failed to load reminder for editing.", ex);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReminderEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                await _reminderService.UpdateReminderAsync(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogError("Failed to update reminder.", ex);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _reminderService.DeleteReminderAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogError("Failed to delete reminder.", ex);
                return RedirectToAction("Index");
            }
        }
    }

}
