using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=RecipeApp;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<Ingredient> Ingredient { get; set; }
        public DbSet<Instruction> Instruction { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ingredient>(b =>
            {
                b.HasOne<Recipe>()
                    .WithMany(x => x.Ingredients)
                    .HasForeignKey(x => x.RecipeId);
            });

			modelBuilder.Entity<Instruction>(b =>
			{
				b.HasOne<Recipe>()
					.WithMany(x => x.Instructions)
					.HasForeignKey(x => x.RecipeId);
			});
		}
    }
}
