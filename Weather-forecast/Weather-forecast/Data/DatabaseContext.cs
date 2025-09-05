using Microsoft.EntityFrameworkCore;
using Weather_forecast.Models;

namespace Weather_forecast.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<History> SearchHistory { get; set; }
    }
}
