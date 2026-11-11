using Microsoft.AspNetCore.Mvc;

namespace Quizapp.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}