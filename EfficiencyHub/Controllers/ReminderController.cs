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

        public ReminderController(ReminderService reminderService, ILogger<BaseController> logger, UserManager<ApplicationUser> userManager)
            : base(logger, userManager)
        {
            _reminderService = reminderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    return Unauthorized();
                }

                var reminders = await _reminderService.GetRemindersForUserAsync(user.Id);
                return View(reminders);
            }
            catch (Exception ex)
            {
                LogError("Failed to load reminders.", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Create(Guid assignmentId)
        {
            var model = new ReminderCreateViewModel
            {
                AssignmentId = assignmentId
            };

            return View(model);
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
