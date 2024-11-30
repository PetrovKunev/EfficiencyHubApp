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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DashboardService _dashboardService;

        public DashboardController(ILogger<BaseController> logger, UserManager<ApplicationUser> userManager, DashboardService dashboardService) : base(logger, userManager)
        {
            _dashboardService = dashboardService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (await _userManager.IsInRoleAsync(currentUser, "Administrator"))
            {
                return RedirectToAction("Index", "AdminDashboard", new { area = "Admin" });
            }

            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            return View(dashboardData);
        }
    }
}
