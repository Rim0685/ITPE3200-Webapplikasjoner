using Microsoft.AspNetCore.Mvc;
using Quizapp.Models;

namespace Quizapp.Controllers
{
    public class ItemController : Controller
    {
        private readonly ItemDbContext _itemDbContext;

        public ItemController(ItemDbContext itemDbContext)
        {
            _itemDbContext = itemDbContext;
        }

        public IActionResult Table()
        {
            List<Item> items = _itemDbContext.Items.ToList();
            return View(items);
        }
    }
}