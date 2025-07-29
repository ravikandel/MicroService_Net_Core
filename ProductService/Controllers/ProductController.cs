using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    public class ProductController(ProductDbContext dbContext) : BaseController
    {
        private readonly ProductDbContext _dbContext = dbContext;

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var productDtos = await _dbContext.Products
                .Select(p => new ProductDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Price = p.Price
                })
                .ToListAsync();

            if (!productDtos.Any())
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "No products found!",
                });
            }

            return Ok(new Response<IEnumerable<ProductDto>>
            {
                StatusCode = EResult.Success,
                Message = "Products retrieved successfully",
                Data = productDtos
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);

            if (product == null)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Product not found!",
                });
            }

            var productDto = new ProductDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price
            };

            return Ok(new Response<ProductDto>
            {
                StatusCode = EResult.Success,
                Message = "Product retrieved successfully.",
                Data = productDto
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductInputDto productInputDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                Name = productInputDto.ProductName,
                Price = productInputDto.Price
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            var newProductDto = new ProductDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price
            };

            return Ok(new Response<ProductDto>
            {
                StatusCode = EResult.Success,
                Message = "Product created successfully.",
                Data = newProductDto
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductInputDto productInputDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProduct = await _dbContext.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Product not found!",
                });
            }

            existingProduct.Name = productInputDto.ProductName;
            existingProduct.Price = productInputDto.Price;

            // No need to call Update explicitly if tracked entity changed
            await _dbContext.SaveChangesAsync();

            var updatedDto = new ProductDto
            {
                ProductId = existingProduct.Id,
                ProductName = existingProduct.Name,
                Price = existingProduct.Price
            };

            return Ok(new Response<ProductDto>
            {
                StatusCode = EResult.Success,
                Message = "Product updated successfully.",
                Data = updatedDto
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);

            if (product == null)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Product not found!",
                });
            }

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            return Ok(new Response
            {
                StatusCode = EResult.Success,
                Message = "Product deleted successfully!",
            });
        }
    }
}
