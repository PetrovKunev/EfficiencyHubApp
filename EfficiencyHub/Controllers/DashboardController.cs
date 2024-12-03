using EfficiencyHub.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EfficiencyHub.Services.Data;

namespace EfficiencyHub.Web.Controllers
{
  [Authorize] 
  public class DashboardController : BaseController
    {
        private readonly new UserManager<ApplicationUser> _userManager;
        private readonly DashboardService _dashboardService;

        public DashboardController(ILogger<BaseController> logger, UserManager<ApplicationUser> userManager, DashboardService dashboardService) : base(logger, userManager)
        {
            _dashboardService = dashboardService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Dashboard Index action started.");

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("Current user is null.");
                return Unauthorized();
            }

            _logger.LogInformation("Current user retrieved: {UserId}", currentUser.Id);

            if (await _userManager.IsInRoleAsync(currentUser, "Administrator"))
            {
                _logger.LogInformation("User is in Administrator role. Redirecting to AdminDashboard.");
                return RedirectToAction("Index", "AdminDashboard", new { area = "Admin" });
            }

            _logger.LogInformation("User is not an Administrator. Fetching dashboard data.");

            var dashboardData = await _dashboardService.GetDashboardDataAsync(currentUser.Id);
            _logger.LogInformation("Dashboard data retrieved successfully.");

            return View(dashboardData);
        }


    }
}
