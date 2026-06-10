using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using polnuyaPetch.Config;
using polnuyaPetch.Data;
using polnuyaPetch.Security;
using polnuyaPetch.Services;
using System;
using System.IO;
using System.Linq;

namespace polnuyaPetch.Controllers
{
    public class CsvController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ImportService _importService;
        private readonly AppConfig _appConfig;

        public CsvController(ApplicationDbContext context, ImportService importService, AppConfig appConfig)
        {
            _context = context;
            _importService = importService;
            _appConfig = appConfig;
        }

        public IActionResult Index()
        {
            try
            {
                AccessControl.RequireAdmin(_appConfig.Role);
                return View();
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Export()
        {
            try
            {
                AccessControl.RequireAdmin(_appConfig.Role);

                string exportsFolder = Path.Combine(AppContext.BaseDirectory, "exports");
                Directory.CreateDirectory(exportsFolder);

                string fileName = $"menu_export_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
                string fullPath = Path.Combine(exportsFolder, fileName);

                var storage = new CsvMenuStorage(fullPath);
                storage.Save(_context.MenuItems.ToList());

                TempData["SuccessMessage"] = $"Экспорт в CSV успешно выполнен! Файл сохранен в папку exports: {fileName}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка экспорта: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Import(IFormFile csvFile, string importMode)
        {
            try
            {
                AccessControl.RequireAdmin(_appConfig.Role);

                if (csvFile == null || csvFile.Length == 0)
                {
                    TempData["ErrorMessage"] = "Вы не выбрали файл для загрузки.";
                    return RedirectToAction("Index");
                }

                string tempPath = Path.GetTempFileName();
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    csvFile.CopyTo(stream);
                }

                var storage = new CsvMenuStorage(tempPath);
                var loaded = storage.Load();

                if (importMode == "Merge")
                {
                    var res = _importService.MergeCsvImport(loaded.items);
                    TempData["SuccessMessage"] = $"Успешное объединение таблиц! Добавлено: {res.added}, пропущено дубликатов: {res.skipped}, ошибок чтения строк: {loaded.errors}";
                }
                else
                {
                    _importService.ReplaceCsvImport(loaded.items);
                    TempData["SuccessMessage"] = $"Полная замена меню выполнена! Импортировано элементов: {loaded.items.Count}, ошибок чтения строк: {loaded.errors}";
                }

                System.IO.File.Delete(tempPath);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка импорта: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
