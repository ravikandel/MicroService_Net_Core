using Microsoft.AspNetCore.Mvc;

namespace ProductService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static readonly List<Product> _products = new List<Product>();

        [HttpGet]
        public IActionResult Get() => Ok(_products);

        [HttpGet("{id}")]
        public ActionResult<Product> Get(int id)
        {
            var product = _products.FirstOrDefault(o => o.Id == id);
            if (product == null) return NotFound("Product not found");
            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            Product product = new Product
            {
                Id = _products.Any() ? _products.Max(o => o.Id) + 1 : 1,
                Name = productDto.Name,
                Price = productDto.Price,
            };

            _products.Add(product);
            return Ok(product);
            // return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }
    }
}