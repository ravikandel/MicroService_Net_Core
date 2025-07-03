public class Order : OrderDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}

public class OrderDto
{
    public required string Name { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
