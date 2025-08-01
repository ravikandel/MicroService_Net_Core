using ProductService.DTOs;
using ProductService.Models;
using ProductService.Repository;

namespace ProductService.Logic
{
    public class ProductLogic(IProductRepository repository) : IProductLogic
    {
        private readonly IProductRepository _repository = repository;

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            return await _repository.GetAllAsync();

        }

        public async Task<ProductDto?> GetAsync(int id)
        {
            return await _repository.GetAsync(id);

        }

        public async Task<ProductDto> CreateAsync(ProductInputDto inputDto)
        {

            Product product = new Product
            {
                Name = inputDto.ProductName,
                Price = inputDto.Price
            };

            var createdProduct = await _repository.CreateAsync(product);

            return new ProductDto
            {
                ProductId = createdProduct.Id,
                ProductName = createdProduct.Name,
                Price = createdProduct.Price
            };

        }

        public async Task<ProductDto?> UpdateAsync(int id, ProductInputDto inputDto)
        {
            var updatedProduct = await _repository.UpdateAsync(id, inputDto);

            if (updatedProduct == null)
                return null;

            return new ProductDto
            {
                ProductId = updatedProduct.Id,
                ProductName = updatedProduct.Name,
                Price = updatedProduct.Price
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}