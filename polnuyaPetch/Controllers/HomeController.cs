using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using polnuyaPetch.Data;
using polnuyaPetch.Models;
using Microsoft.AspNetCore.Http;
using polnuyaPetch.Config;
using polnuyaPetch.Security;

namespace polnuyaPetch.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ConfigService _configService;
        private readonly AppConfig _appConfig;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment env, ConfigService configService, AppConfig appConfig)
        {
            _logger = logger;
            _context = context;
            _env = env;
            _configService = configService;
            _appConfig = appConfig;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> Menu(string? searchTerm, string[]? categories)
        {
            var allItems = await _context.MenuItems.ToListAsync();
            var query = allItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim();
                query = query.Where(m => m.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (categories != null && categories.Length > 0)
            {
                query = query.Where(m => categories.Contains(m.Category));
            }

            return View(query.ToList());
        }

        public IActionResult Reservation() => View();

        [HttpPost]
        public async Task<IActionResult> Reservation(Reservation booking)
        {
            if (ModelState.IsValid)
            {
                _context.Reservations.Add(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Столик успешно забронирован!";
                return RedirectToAction("Reservation");
            }
            return View(booking);
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Login == user.Login))
                {
                    ModelState.AddModelError("Login", "Этот логин уже занят");
                    return View(user);
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserLogin", user.Login);

                if (string.Equals(user.Login, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    _appConfig.Role = "Admin";
                }
                else
                {
                    _appConfig.Role = "User";
                }
                _configService.Save(_appConfig);

                return RedirectToAction("Profile");
            }
            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var login = HttpContext.Session.GetString("UserLogin");
            if (string.IsNullOrEmpty(login)) return RedirectToAction("Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            ViewBag.Login = user?.Login;
            ViewBag.AvatarPath = user?.AvatarPath ?? "/images/default-avatar.png";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatar)
        {
            var login = HttpContext.Session.GetString("UserLogin");
            if (avatar != null && login != null)
            {
                var avatarsDir = Path.Combine(_env.WebRootPath, "avatars");
                if (!Directory.Exists(avatarsDir)) Directory.CreateDirectory(avatarsDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatar.FileName);
                var filePath = Path.Combine(avatarsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(stream);
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
                if (user != null)
                {
                    user.AvatarPath = "/avatars/" + fileName;
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Profile");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            _appConfig.Role = "User";
            _configService.Save(_appConfig);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AdminReservations()
        {
            try
            {
                AccessControl.RequireAdmin(_appConfig.Role);
                var reservations = await _context.Reservations
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
                return View(reservations);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                AccessControl.RequireAdmin(_appConfig.Role);
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation != null)
                {
                    _context.Reservations.Remove(reservation);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("AdminReservations");
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult Connection() => View();

        public IActionResult Addresses() => View();

        public IActionResult Settings()
        {
            try
            {
                AccessControl.RequireAdmin(_appConfig.Role);
                return View(_appConfig);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Settings(string storageMode, bool askOnStart, string role)
        {
            try
            {
                AccessControl.RequireAdmin(_appConfig.Role);
                _appConfig.StorageMode = storageMode;
                _appConfig.AskOnStart = askOnStart;
                _appConfig.Role = role;

                _configService.Save(_appConfig);

                TempData["SuccessMessage"] = $"Настройки успешно сохранены! Текущая роль в системе: {role}.";
                return RedirectToAction("Settings");
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
