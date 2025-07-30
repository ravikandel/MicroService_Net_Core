using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public string? OrderName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderStaus { get; set; }

    }
    public class OrderDto : OrderResponseDto
    {
        public List<OrderDetailDto> OrderDetails { get; set; } = [];

    }

    public class OrderDetailDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }  // Will be fetched via HTTP
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }

    public class OrderInputDto
    {
        [Required]
        public string? OrderName { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required.")]
        public required List<OrderDetailInputDto> OrderDetails { get; set; }

    }

    public class OrderDetailInputDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

    }

    public class OrderUpdateDto
    {
        [Required]
        public string? OrderName { get; set; }

        [Required]
        public int OrderStaus { get; set; }

    }

}
