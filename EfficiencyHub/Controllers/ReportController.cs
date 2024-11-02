using Microsoft.AspNetCore.Mvc;

namespace EfficiencyHub.Web.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
