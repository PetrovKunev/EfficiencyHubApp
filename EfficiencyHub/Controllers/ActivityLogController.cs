using EfficiencyHub.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EfficiencyHub.Services.Data;


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
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                
                var totalLogs = await _activityLogService.GetTotalLogsAsync(currentUser.Id);
                var totalPages = (int)Math.Ceiling(totalLogs / (double)pageSize);

                var activityLogs = await _activityLogService.GetPagedUserActionsAsync(currentUser.Id, pageNumber, pageSize);

                ViewData["CurrentPage"] = pageNumber;
                ViewData["PageSize"] = pageSize;
                ViewData["TotalPages"] = totalPages;

                return View(activityLogs);
            }
            catch (Exception ex)
            {
                LogError("Error loading activity logs.", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? searchTerm = null, int pageNumber = 1, int pageSize = 10)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                var (logs, totalCount) = await _activityLogService.SearchActivityLogsAsync(
                    currentUser.Id, searchTerm, pageNumber, pageSize);

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                ViewData["SearchTerm"] = searchTerm;
                ViewData["CurrentPage"] = pageNumber;
                ViewData["PageSize"] = pageSize;
                ViewData["TotalPages"] = totalPages;

                return View("Index", logs);
            }
            catch (Exception ex)
            {
                LogError("Error while searching activity logs.", ex);
                return RedirectToAction("Error", "Home");
            }
        }

    }
}