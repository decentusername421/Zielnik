using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Zielnik.Controllers
{
    
    public class StatsPageController : Controller
    {
        
        public IActionResult Index()
        {
            return View(); 
        }
    }
}