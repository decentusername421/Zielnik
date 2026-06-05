using Microsoft.AspNetCore.Mvc;

namespace Zielnik.Controllers
{
    public class AccountController : Controller 
    {
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}