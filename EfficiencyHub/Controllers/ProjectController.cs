using Microsoft.AspNetCore.Mvc;

namespace EfficiencyHub.Web.Controllers
{
    public class ProjectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
