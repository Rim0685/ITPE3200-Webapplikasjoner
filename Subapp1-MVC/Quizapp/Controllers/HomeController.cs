using Microsoft.AspNetCore.Mvc;
namespace QuizApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}