using Microsoft.AspNetCore.Mvc;

namespace EfficiencyHub.Web.Controllers
{
    public class ReminderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
