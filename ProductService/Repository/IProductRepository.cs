
using ProductService.DTOs;
using ProductService.Models;

namespace ProductService.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(int id, ProductInputDto inputDto);
        Task<bool> DeleteAsync(int id);
    }
}