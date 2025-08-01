using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Common;
using ProductService.DTOs;
using ProductService.Logic;

namespace ProductService.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    public class ProductController(IProductLogic logic) : BaseController
    {
        private readonly IProductLogic _logic = logic;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var productDtos = await _logic.GetAllAsync();

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
                Message = "Products retrieved successfully!",
                Data = productDtos
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var productDto = await _logic.GetAsync(id);

            if (productDto == null)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Product not found!",
                });
            }

            return Ok(new Response<ProductDto>
            {
                StatusCode = EResult.Success,
                Message = "Product retrieved successfully!",
                Data = productDto
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductInputDto productInputDto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Product information are required!",
                });
            }

            var createdProduct = await _logic.CreateAsync(productInputDto);

            return Ok(new Response<ProductDto>
            {
                StatusCode = EResult.Success,
                Message = "Product created successfully!",
                Data = createdProduct
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductInputDto productInputDto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Product information are required!",
                });
            }
            var updatedProduct = await _logic.UpdateAsync(id, productInputDto);

            if (updatedProduct == null)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Product not found!"
                });
            }

            return Ok(new Response<ProductDto>
            {
                StatusCode = EResult.Success,
                Message = "Product updated successfully!",
                Data = updatedProduct
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _logic.DeleteAsync(id);

            if (!result)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Product not found!",
                });
            }

            return Ok(new Response
            {
                StatusCode = EResult.Success,
                Message = "Product deleted successfully!"
            });
        }
    }
}
