using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication4.Models;

namespace UnitTestProject1
{
    [TestClass]
    public class RecipeTests
    {
        [TestMethod]
        public void AddRecipe_WithEggIngredient_ShouldBeSavedToDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Create a temporary context for setup
            using (var context = new ApplicationDbContext(options))
            {
                var eggCategory = new Category
                {
                    CategoryId = 1,
                    Name = "Egg & Milk Based Products"
                };

                var eggIngredient = new Ingredient
                {
                    IngredientId = 1,
                    Name = "Egg",
                    Category = eggCategory
                };

                context.Categories.Add(eggCategory);
                context.Ingredients.Add(eggIngredient);
                context.SaveChanges();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var recipe = new Recipe
                {
                    RecipeId = 1,
                    Title = "Egg Recipe",
                    Description = "A simple egg recipe",
                    RecipeIngredients = new System.Collections.Generic.List<RecipeIngredient>
                    {
                        new RecipeIngredient
                        {
                            IngredientId = 1 // Linking to the Egg ingredient
                        }
                    }
                };

                context.Recipes.Add(recipe);
                context.SaveChanges();
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var savedRecipe = context.Recipes
                    .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                    .FirstOrDefault();

                Assert.IsNotNull(savedRecipe);
                Assert.AreEqual("Egg Recipe", savedRecipe.Title);
                Assert.AreEqual(1, savedRecipe.RecipeIngredients.Count);
                Assert.AreEqual("Egg", savedRecipe.RecipeIngredients.First().Ingredient.Name);
            }
        }
    }
}
