using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using WebApplication4.Models;

namespace TestProject2
{
    [TestClass]
    public class RecipeControllerTests
    {
        [TestMethod]
        public void CreateRecipe_ShouldAssociateIngredientsCorrectly()
        {
            // Arrange
            // Simulating in-memory database collections
            var ingredients = new List<Ingredient>
            {
                new Ingredient { IngredientId = 1, Name = "Egg", Category = new Category { Name = "Egg & Milk Based Products" } },
                new Ingredient { IngredientId = 2, Name = "Milk", Category = new Category { Name = "Egg & Milk Based Products" } }
            };

            var recipes = new List<Recipe>();
            var recipeIngredients = new List<RecipeIngredient>();

            // The recipe to add
            var recipe = new Recipe
            {
                RecipeId = 1,
                Title = "Egg Recipe",
                Description = "A delicious egg recipe."
            };

            var ingredientIds = new List<int> { 1 }; // IDs of ingredients to associate (e.g., Egg)

            // Act
            // Fetch selected ingredients
            var selectedIngredients = ingredients
                                       .Where(i => ingredientIds.Contains(i.IngredientId))
                                       .ToList();

            // Create RecipeIngredients for each selected ingredient
            var newRecipeIngredients = selectedIngredients
                .Select(i => new RecipeIngredient
                {
                    Ingredient = i,
                    Recipe = recipe
                })
                .ToList();

            // Simulate adding RecipeIngredients to the recipe
            recipe.RecipeIngredients = newRecipeIngredients;

            // Simulate saving to the database
            recipes.Add(recipe);
            recipeIngredients.AddRange(newRecipeIngredients);

            // Assert
            Assert.AreEqual(1, recipes.Count, "The recipe should be added to the collection.");
            Assert.AreEqual(1, recipe.RecipeIngredients.Count, "The recipe should have one associated ingredient.");
            Assert.AreEqual("Egg", recipe.RecipeIngredients.First().Ingredient.Name);
            Assert.AreEqual("Egg & Milk Based Products", recipe.RecipeIngredients.First().Ingredient.Category.Name);
        }
    }
}
