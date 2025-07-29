using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public int OrderStaus { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = [];
    }

    public class OrderDetail
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Order? Order { get; set; }
    }
}