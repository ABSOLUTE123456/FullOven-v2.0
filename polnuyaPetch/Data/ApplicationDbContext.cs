using Microsoft.EntityFrameworkCore;
using polnuyaPetch.Models;
using System.Collections.Generic;

namespace polnuyaPetch.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Исправлено: S должна быть заглавной (DbSet)
        public DbSet<MenuItem> MenuItems { get; set; }
    }
}