using EfficiencyHub.Data.Models;
using EfficiencyHub.Services.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EfficiencyHub.Web.Controllers
{
    [Authorize]
    public class PerformanceReportController : BaseController
    {
        private readonly PerformanceReportService _performanceReportService;

        public PerformanceReportController(
            ILogger<PerformanceReportController> logger,
            UserManager<ApplicationUser> userManager,
            PerformanceReportService performanceReportService)
            : base(logger, userManager)
        {
            _performanceReportService = performanceReportService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            
            if (!startDate.HasValue || !endDate.HasValue)
            {
                startDate = DateTime.UtcNow.AddDays(-7);
                endDate = DateTime.UtcNow;
            }

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                _logger.LogError("User not logged in.");
                return RedirectToAction("Login", "Account");
            }

            
            var report = await _performanceReportService.GetPerformanceReportAsync(user.Id, startDate.Value, endDate.Value);

            
            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

            return View(report);
        }
    }
}

