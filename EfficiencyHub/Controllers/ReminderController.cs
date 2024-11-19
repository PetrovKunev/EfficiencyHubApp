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
        public async Task<IActionResult> Create(Guid assignmentId)
        {
            // Вземаме текущия потребител
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            // Проверяваме дали можем да вземем име на задачата
            var assignmentName = await _assignmentService.GetAssignmentNameAsync(assignmentId);
            if (string.IsNullOrEmpty(assignmentName))
            {
                return NotFound("Assignment not found.");
            }

            // Подготвяме модела
            ViewBag.AssignmentId = assignmentId;
            ViewBag.AssignmentName = assignmentName;

            var model = new ReminderCreateViewModel
            {
                AssignmentId = assignmentId,
                ReminderDate = DateTime.Now.AddDays(1)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReminderCreateViewModel model)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                
                await _reminderService.CreateReminderAsync(model, currentUser.Id);
                return RedirectToAction("Index", new { assignmentId = model.AssignmentId });
            }
            catch (Exception ex)
            {
                LogError("Error creating reminder.", ex);
                return RedirectToAction("Error", "Home");
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
