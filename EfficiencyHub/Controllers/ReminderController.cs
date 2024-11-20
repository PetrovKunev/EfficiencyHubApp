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

            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }


            var assignmentName = await _assignmentService.GetAssignmentNameAsync(assignmentId);
            if (string.IsNullOrEmpty(assignmentName))
            {
                return NotFound("Assignment not found.");
            }


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
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                // Зареждане на напомнянието
                var reminder = await _reminderService.GetReminderByIdAsync(id, currentUser.Id);
                if (reminder == null)
                {
                    _logger.LogError($"Reminder with Id {id} not found.");
                    return NotFound("Reminder not found.");
                }

                if (reminder.AssignmentId == Guid.Empty)
                {
                    _logger.LogError($"Reminder with Id {id} has an invalid AssignmentId.");
                    return NotFound("Reminder is not associated with a valid assignment.");
                }

                // Зареждане на ProjectId чрез AssignmentId
                var projectId = await _assignmentService.GetProjectIdByAssignmentAsync(reminder.AssignmentId);
                ViewBag.ProjectId = projectId;

                // Зареждане на името на задачата
                var assignmentName = await _assignmentService.GetAssignmentNameAsync(reminder.AssignmentId);
                ViewBag.AssignmentName = assignmentName;

                // Преобразуване на модела към ReminderEditViewModel
                var editModel = new ReminderEditViewModel
                {
                    Id = reminder.Id,
                    AssignmentId = reminder.AssignmentId,
                    Message = reminder.Message,
                    ReminderDate = reminder.ReminderDate
                };

                return View(editModel); // Изпращаме ReminderEditViewModel към изгледа
            }
            catch (InvalidOperationException ex)
            {
                LogError("Error loading project or assignment.", ex);
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReminderEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                await _reminderService.UpdateReminderAsync(model, currentUser.Id);
                return RedirectToAction("Index", new { assignmentId = model.AssignmentId });
            }
            catch (Exception ex)
            {
                LogError("Error updating reminder.", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                var assignmentId = await _reminderService.DeleteReminderAsync(id, currentUser.Id);
                if (assignmentId == null)
                {
                    TempData["ErrorMessage"] = "Reminder not found or you are not authorized to delete it.";
                    return RedirectToAction("Index");
                }

                TempData["SuccessMessage"] = "Reminder successfully deleted.";
                return RedirectToAction("Index", new { assignmentId });
            }
            catch (Exception ex)
            {
                LogError("Error deleting reminder.", ex);
                TempData["ErrorMessage"] = "An error occurred while deleting the reminder.";
                return RedirectToAction("Index");
            }
        }


    }

}
