using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationFoodManagerDbContext _context;

        public RecipesController(ApplicationFoodManagerDbContext context)
        {
            _context = context;
        }

        // GET: Recipes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Recipes.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)  // Include RecipeIngredients
                    .ThenInclude(ri => ri.Ingredient)  // Include the related Ingredient via RecipeIngredients
                .FirstOrDefaultAsync(m => m.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }



        // GET: Recipes/Create
        public IActionResult Create()
        {
            // If the user is not logged in as an admin, redirect to login
            var adminministratorId = HttpContext.Session.GetInt32("AdministratorId");
            if (adminministratorId == null)
            {
                return RedirectToAction("AdminLogin", "Home"); // Redirect to login if not logged in
            }

            // Fetch all categories from the database
            var categories = _context.Categories.ToList();

            if (categories == null || !categories.Any())
            {
                // Handle the case where no categories are available
                ViewBag.Categories = new List<Category>(); // Pass an empty list to avoid null reference
            }
            else
            {
                ViewBag.Categories = categories;
            }

            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Save the recipe with selected ingredients
        // POST: Recipes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Recipe recipe, List<int> ingredientIds)
        {

            // Check if user is logged in
            var adminId = HttpContext.Session.GetInt32("AdministratorId");
            if (adminId == null)
            {
                return RedirectToAction("AdminLogin", "Home"); // Redirect to login if not logged in
            }

            // Fetch the user and recipe
            var admin = _context.Administrators.Include(a => a.AddedRecipes).FirstOrDefault(a => a.Id == adminId);

            // Fetch selected ingredients
            var selectedIngredients = _context.Ingredients
                                               .Where(i => ingredientIds.Contains(i.IngredientId))
                                               .ToList();

            // Create RecipeIngredients for each selected ingredient
            var recipeIngredients = selectedIngredients
                .Select(i => new RecipeIngredient
                {
                    Ingredient = i,
                    Recipe = recipe
                })
                .ToList();

            // Add RecipeIngredients to the recipe's RecipeIngredients collection
            recipe.RecipeIngredients = recipeIngredients;

            // Add the recipe to the database
            _context.Recipes.Add(recipe);
            admin.AddedRecipes.Add(recipe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Recipes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Recipe recipe, List<int> ingredientIds)
        {
            if (id != recipe.RecipeId)
            {
                return NotFound();
            }
            try
            {
                // Retrieve the existing recipe with its RecipeIngredients
                var existingRecipe = await _context.Recipes
                    .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient) // Include Ingredients
                    .FirstOrDefaultAsync(r => r.RecipeId == id);

                if (existingRecipe == null)
                {
                    return NotFound();
                }

                // Update fields
                existingRecipe.Title = recipe.Title;
                existingRecipe.Description = recipe.Description;
                existingRecipe.Steps = recipe.Steps;

                // Update ingredients
                if (ingredientIds != null)
                {
                    // Fetch selected ingredients
                    var selectedIngredients = _context.Ingredients
                                                       .Where(i => ingredientIds.Contains(i.IngredientId))
                                                       .ToList();

                    // Create new RecipeIngredient entries
                    existingRecipe.RecipeIngredients = selectedIngredients
                        .Select(i => new RecipeIngredient
                        {
                            Recipe = existingRecipe,
                            Ingredient = i
                        })
                        .ToList();
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(recipe.RecipeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }



        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.RecipeId == id);
        }

        // GET: Fetch ingredients by category
        public IActionResult GetIngredientsByCategory(int categoryId)
        {
            var ingredients = _context.Ingredients
                                      .Where(i => i.CategoryId == categoryId)
                                      .ToList();
            return Json(ingredients); // Return ingredients as JSON
        }

        [HttpGet]
        public async Task<IActionResult> GetIngredientSuggestions(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Json(new List<string>()); // Return empty list if query is too short
            }

            // Fetch top 5 ingredients starting with the query
            var suggestions = await _context.Ingredients
                .Where(i => EF.Functions.Like(i.Name, $"{query}%")) // Matches beginning of the name
                .OrderBy(i => i.Name)
                .Select(i => i.Name)
                .Take(5)
                .ToListAsync();

            return Json(suggestions);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByIngredients(string ingredientName)
        {
            if (string.IsNullOrWhiteSpace(ingredientName))
            {
                return View(new List<Recipe>()); // Return empty list if no ingredient provided
            }

            // Fetch recipes where any ingredient name matches the query
            var recipes = await _context.Recipes
                .Where(r => r.RecipeIngredients
                    .Any(ri => EF.Functions.Like(ri.Ingredient.Name, $"%{ingredientName}%"))) // Fix here: Access Ingredient.Name
                .Include(r => r.RecipeIngredients) // Include RecipeIngredients for detailed results
                .ThenInclude(ri => ri.Ingredient) // Include the Ingredient associated with RecipeIngredients
                .ToListAsync();

            return View(recipes);
        }


        [HttpGet]
        public async Task<IActionResult> SearchByRecipeName(string recipeName)
        {
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return View(new List<Recipe>());
            }

            // Fetching recipes that match the name
            var recipes = await _context.Recipes
                .Where(r => EF.Functions.Like(r.Title, $"%{recipeName}%"))
                .ToListAsync();  // Return full Recipe objects

            return View(recipes);  // Pass the list of Recipe objects to the view
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToFavorites(int recipeId)
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Home"); // Redirect to login if not logged in
            }

            // Fetch the user and recipe
            var user = _context.Users.Include(u => u.FavorteRecipes).FirstOrDefault(u => u.Id == userId);
            var recipe = _context.Recipes.FirstOrDefault(r => r.RecipeId == recipeId);

            if (user == null || recipe == null)
            {
                return NotFound(); // Handle not found cases
            }

            // Add the recipe to the user's favorites
            if (!user.FavorteRecipes.Contains(recipe))
            {
                user.FavorteRecipes.Add(recipe);
                _context.SaveChanges(); // Save changes
            }

            return RedirectToAction("Details", "Recipes", new { id = recipeId }); // Redirect to recipe details page
        }

        [HttpPost]
        public IActionResult RemoveFromFavorites(int recipeId)
        {
            // Get the logged-in user ID from the session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized(); // Ensure the user is logged in
            }

            // Retrieve the user and the recipe
            var user = _context.Users
                .Include(u => u.FavorteRecipes)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var recipe = user.FavorteRecipes.FirstOrDefault(r => r.RecipeId == recipeId);
            if (recipe != null)
            {
                user.FavorteRecipes.Remove(recipe); // Remove the recipe from the list
                _context.SaveChanges(); // Save changes to the database
            }

            return RedirectToAction("Profile", "Home");

        }



    }
}
