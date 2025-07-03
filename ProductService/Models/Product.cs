public class Product : ProductDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
}

public class ProductDto {
    public required string Name { get; set; }
    public decimal Price { get; set; }
}