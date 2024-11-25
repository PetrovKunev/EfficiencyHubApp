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
        private readonly DashboardService _dashboardService;

        public DashboardController(ILogger<BaseController> logger, UserManager<ApplicationUser> userManager, DashboardService dashboardService) : base(logger, userManager)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            return View(dashboardData);
        }

       
    }
}
