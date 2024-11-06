using EfficiencyHub.Data.Models;
using EfficiencyHub.Web.Controllers;
using EfficiencyHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EfficiencyHub.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(ILogger<BaseController> logger, UserManager<ApplicationUser> userManager)
            : base(logger, userManager)
        {
        }

        public async Task<IActionResult> LandingPage()
        {
            var currentUser = await GetCurrentUserAsync();
            // Ако се нуждаете от данни за текущия потребител, можете да ги използвате тук
            return View();
        }

        public async Task<IActionResult> About()
        {
            var currentUser = await GetCurrentUserAsync();
            // Добавете допълнителна логика, ако трябва да използвате данни за текущия потребител
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}