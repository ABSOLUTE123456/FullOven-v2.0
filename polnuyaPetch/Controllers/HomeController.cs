using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using polnuyaPetch.Data;
using polnuyaPetch.Models;

namespace polnuyaPetch.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index() => View();

        // ЖЕЛЕЗОБЕТОННЫЙ МЕТОД МЕНЮ
        public async Task<IActionResult> Menu(string? searchTerm, string[]? categories)
        {
            // 1. Сначала забираем ВСЕ блюда из базы в память (решает проблему кодировки)
            var allItems = await _context.MenuItems.ToListAsync();

            // Превращаем в перечисление для фильтрации силами C#
            var query = allItems.AsEnumerable();

            // 2. Поиск по названию (теперь работает идеально с любым регистром)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim();
                query = query.Where(m => m.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // 3. Фильтр по категориям
            if (categories != null && categories.Length > 0)
            {
                query = query.Where(m => categories.Contains(m.Category));
            }

            return View(query.ToList());
        }

        public IActionResult Reservation() => View();

        public IActionResult Connection() => View();

        public IActionResult Addresses() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
