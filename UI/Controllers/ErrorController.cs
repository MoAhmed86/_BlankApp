using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult E403()
        {
            return View();
        }
        public IActionResult E404()
        {
            return View("E404");
        }

        public IActionResult ProjectReceived(int id)
        { 
            return View(id);
        }

        public IActionResult E500()
        {
            return View();
        }
    }
}