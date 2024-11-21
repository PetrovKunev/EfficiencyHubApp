using EfficiencyHub.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace EfficiencyHub.Web.Controllers
{
    [Authorize]
    public class ActivityLogController : BaseController
    {
        private readonly ActivityLogService _activityLogService;

        public ActivityLogController(ActivityLogService activityLogService, ILogger<BaseController> logger, UserManager<ApplicationUser> userManager)
            : base(logger, userManager)
        {
            _activityLogService = activityLogService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                var activityLogs = await _activityLogService.GetLastUserActionsAsync(currentUser.Id);
                return View(activityLogs);
            }
            catch (Exception ex)
            {
                LogError("Error loading activity logs.", ex);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}