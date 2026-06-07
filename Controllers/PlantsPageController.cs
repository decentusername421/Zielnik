using Microsoft.AspNetCore.Mvc;

namespace Zielnik.Controllers
{
    public class PlantsPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}