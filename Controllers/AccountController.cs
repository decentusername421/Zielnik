using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Zielnik.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet("check-role")]
        [Authorize]
        public IActionResult CheckRole()
        {
            return Ok(new { isAdmin = User.IsInRole("Admin") });
        }


        // Wyświetlanie strony logowania
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Wyświetlanie strony rejestracji
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

    }
}
