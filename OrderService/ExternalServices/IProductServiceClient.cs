using OrderService.DTOs;

namespace OrderService.ExternalServices
{
    public interface IProductServiceClient
    {
        Task<ProductDto?> GetProductAsync(int productId);
    }
}