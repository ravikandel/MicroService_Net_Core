using System.ComponentModel.DataAnnotations;

namespace ProductService.Models
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; } 
        public decimal Price { get; set; }
    }

    public class ProductInputDto
    {
        [Required]
        public string? ProductName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }

}
