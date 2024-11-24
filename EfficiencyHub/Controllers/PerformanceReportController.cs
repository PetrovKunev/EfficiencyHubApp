using EfficiencyHub.Data.Models;
using EfficiencyHub.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EfficiencyHub.Web.Controllers
{
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
            _logger.LogInformation("Received StartDate: {StartDate}, EndDate: {EndDate}", startDate, endDate);

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

            return View(report);
        }


    }
}
