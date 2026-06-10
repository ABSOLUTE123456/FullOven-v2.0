using System;
using System.Collections.Generic;
using System.Linq;
using polnuyaPetch.Data;
using polnuyaPetch.Models;

namespace polnuyaPetch.Services
{
    public class ImportService
    {
        private readonly ApplicationDbContext _context;

        public ImportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void ReplaceImport(List<MenuItem> importedItems)
        {
            if (importedItems == null) return;
            _context.MenuItems.RemoveRange(_context.MenuItems);
            _context.SaveChanges();
            foreach (var item in importedItems)
            {
                item.Id = 0;
                _context.MenuItems.Add(item);
            }
            _context.SaveChanges();
        }

        public (int added, int skipped) MergeImport(List<MenuItem> importedItems)
        {
            if (importedItems == null) return (0, 0);
            int added = 0;
            int skipped = 0;
            var currentItems = _context.MenuItems.ToList();

            foreach (var item in importedItems)
            {
                if (item == null) { skipped++; continue; }

                bool isDuplicate = currentItems.Any(x =>
                    string.Equals(x.Name, item.Name, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(x.Category, item.Category, StringComparison.OrdinalIgnoreCase)
                );

                if (isDuplicate) { skipped++; continue; }

                _context.MenuItems.Add(new MenuItem
                {
                    Name = item.Name ?? "",
                    Category = item.Category ?? "",
                    Price = item.Price,
                    ImagePath = item.ImagePath ?? "",
                    Description = item.Description ?? ""
                });
                added++;
            }
            _context.SaveChanges();
            return (added, skipped);
        }

        public void ReplaceCsvImport(List<MenuItem> importedItems)
        {
            if (importedItems == null) return;
            _context.MenuItems.RemoveRange(_context.MenuItems);
            _context.SaveChanges();
            foreach (var item in importedItems)
            {
                item.Id = 0;
                _context.MenuItems.Add(item);
            }
            _context.SaveChanges();
        }

        public (int added, int skipped) MergeCsvImport(List<MenuItem> importedItems)
        {
            if (importedItems == null) return (0, 0);
            int added = 0;
            int skipped = 0;
            var currentItems = _context.MenuItems.ToList();

            foreach (var item in importedItems)
            {
                if (item == null) { skipped++; continue; }
                bool isDuplicate = currentItems.Any(x => string.Equals(x.Name, item.Name, StringComparison.OrdinalIgnoreCase));
                if (isDuplicate) { skipped++; continue; }

                item.Id = 0;
                _context.MenuItems.Add(item);
                added++;
            }
            _context.SaveChanges();
            return (added, skipped);
        }
    }
}
