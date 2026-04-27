using Microsoft.EntityFrameworkCore;
using polnuyaPetch.Models;

namespace polnuyaPetch.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<User> Users { get; set; }

    }
}
