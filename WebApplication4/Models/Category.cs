using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
