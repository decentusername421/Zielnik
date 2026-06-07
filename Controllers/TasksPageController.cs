using Microsoft.AspNetCore.Mvc;

namespace Zielnik.Controllers
{
    public class TasksPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}