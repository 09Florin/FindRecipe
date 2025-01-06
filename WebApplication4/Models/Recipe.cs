namespace WebApplication4.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        // Steps as a list of strings, each item represents a single step
        public List<string> Steps { get; set; } = new List<string>();
        // Navigation property for Ingredients
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }
}
