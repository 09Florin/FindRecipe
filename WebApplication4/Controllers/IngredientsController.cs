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
    public class IngredientsController : Controller
    {
        private readonly ApplicationFoodManagerDbContext _context;

        public IngredientsController(ApplicationFoodManagerDbContext context)
        {
            _context = context;
        }

        // GET: Ingredients
        public async Task<IActionResult> Index()
        {
            // If the user is not logged in as an admin, redirect to login
            var adminministratorId = HttpContext.Session.GetInt32("AdministratorId");
            if (adminministratorId == null)
            {
                return RedirectToAction("AdminLogin", "Home"); // Redirect to login if not logged in
            }

            var applicationDbContext = _context.Ingredients.Include(i => i.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Ingredients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.IngredientId == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        // GET: Ingredients/Create
        public IActionResult Create()
        {
            // If the user is not logged in as an admin, redirect to login
            var adminministratorId = HttpContext.Session.GetInt32("AdministratorId");
            if (adminministratorId == null)
            {
                return RedirectToAction("AdminLogin", "Home"); // Redirect to login if not logged in
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Ingredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CategoryId")] Ingredient ingredient, IFormFile? photo)
        {
            if (photo != null && photo.Length > 0)
            {
                // Save the uploaded photo
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photo.FileName);
                string uploadsFolder = Path.Combine("wwwroot/IngredientsImages/Ingredients");  // Folder where images are stored
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }

                // Save the relative path (from the root of the website)
                ingredient.PhotoPath = $"/IngredientsImages/Ingredients/{uniqueFileName}";
            }
            else
            {
                ingredient.PhotoPath = $"/IngredientsImages/Default/top-view-natural-medicines.jpg";  // Default image
            }

            _context.Add(ingredient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Ingredients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            // Populate the dropdown for Categories
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", ingredient.CategoryId);

            return View(ingredient);
        }



        // POST: Ingredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ingredient ingredient, IFormFile? photo)
        {
            if (id != ingredient.IngredientId)
            {
                ingredient.IngredientId = id;
            }

            try
            {
                // Fetch the existing ingredient from the database
                var existingIngredient = await _context.Ingredients.FindAsync(id);

                if (existingIngredient == null)
                {
                    return NotFound();
                }

                // Update fields other than the photo
                existingIngredient.Name = ingredient.Name;
                existingIngredient.CategoryId = ingredient.CategoryId;

                // Handle photo upload only if a new file is provided
                if (photo != null && photo.Length > 0)
                {
                    // Generate new file path
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photo.FileName);
                    string uploadsFolder = Path.Combine("wwwroot/IngredientsImages/Ingredients");
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the new file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(fileStream);
                    }

                    // Delete the old file if it exists and isn't the default image
                    if (!string.IsNullOrEmpty(existingIngredient.PhotoPath) &&
                        existingIngredient.PhotoPath != "/IngredientsImages/Default/top-view-natural-medicines.jpg")
                    {
                        string oldFilePath = Path.Combine("wwwroot", existingIngredient.PhotoPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Update the photo path in the existing record
                    existingIngredient.PhotoPath = $"/IngredientsImages/Ingredients/{uniqueFileName}";
                }

                // Save changes to the database
                _context.Update(existingIngredient);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientExists(ingredient.IngredientId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: Ingredients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.IngredientId == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        // POST: Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(e => e.IngredientId == id);
        }
    }
}
