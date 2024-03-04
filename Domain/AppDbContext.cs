using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Data\\Employees.db");
        }

        public DbSet<Recipe> Recipes { get; set; }
    }
}
