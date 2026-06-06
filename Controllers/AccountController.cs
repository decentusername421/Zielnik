using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zielnik.DTOs;

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

        [HttpPost("api/account/login")]
        public async Task<IActionResult> LoginApi([FromBody] LoginDto model)
        {

            return Ok(new { token = "twój_wygenerowany_token" });
        }

        [HttpPost("api/account/register")]
        public async Task<IActionResult> RegisterApi([FromBody] RegisterDto model)
        {
            return Ok(new { message = "Konto założone" });
        }
    }
}