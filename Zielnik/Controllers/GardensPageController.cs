using Microsoft.AspNetCore.Mvc;

namespace Zielnik.Controllers
{
    public class GardensPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(Guid id)
        {
            ViewBag.GardenId = id;
            return View();
        }
    }

}
