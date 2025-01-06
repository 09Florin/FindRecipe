using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public string PhotoPath { get; set; }
        // Foreign Key for Category
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }
}
