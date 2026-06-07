using Microsoft.AspNetCore.Mvc;

namespace Zielnik.Controllers
{
    public class StartController : Controller
    {
        // Ta metoda odpowiada za wejście na główną stronę aplikacji
        public IActionResult Index()
        {
            // Jeśli użytkownik jest już zalogowany, wyślij go prosto do Dashboardu
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(); // Jeśli nie, pokazujemy stronę "Start"
        }
    }
}
