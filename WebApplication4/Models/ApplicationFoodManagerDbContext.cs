using Microsoft.EntityFrameworkCore;
using WebApplication4.Models;

namespace WebApplication4.Models
{
    public class ApplicationFoodManagerDbContext : DbContext
    {
        public ApplicationFoodManagerDbContext(DbContextOptions<ApplicationFoodManagerDbContext> options) : base(options)
        {
        }

        // DbSet properties to represent the tables in the database
        public DbSet<User> Users { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Call the base method for other configurations

            // Define composite primary key for RecipeIngredient
            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

            // Configure relationships
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId);
        }
        public DbSet<WebApplication4.Models.Administrator> Administrator { get; set; } = default!;
    }
}
