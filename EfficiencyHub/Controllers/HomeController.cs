using EfficiencyHub.Data.Models;
using EfficiencyHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EfficiencyHub.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(ILogger<BaseController> logger, UserManager<ApplicationUser> userManager)
            : base(logger, userManager)
        {
        }

        [AllowAnonymous]
        public async Task<IActionResult> LandingPage()
        {
            var currentUser = await GetCurrentUserAsync();
            
            return View();
        }

        public async Task<IActionResult> About()
        {
            var currentUser = await GetCurrentUserAsync();
            
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/Home/HandleError/{code:int}")]
        public IActionResult HandleError(int code)
        {
            
            if (code == 404)
            {
                return View("Error404");
            }
                       
            ViewData["ErrorMessage"] = $"Error occurred. The ErrorCode is: {code}";
            return View("GenericError");
        }

        [Route("/Home/Handle500")]
        public IActionResult Handle500()
        {
            return View("Error500");
        }
        public IActionResult Simulate500()
        {
            throw new Exception("This is a simulated 500 error.");
        }
    }

}