using ProductService.DTOs;

namespace ProductService.Logic
{
    public interface IProductLogic
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetAsync(int id);
        Task<ProductDto> CreateAsync(ProductInputDto inputDto);
        Task<ProductDto?> UpdateAsync(int id, ProductInputDto inputDto);
        Task<bool> DeleteAsync(int id);

    }
}