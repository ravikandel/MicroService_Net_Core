using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private static readonly List<Order> _orders = new List<Order>();

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiGatewayBaseUrl;

        // Constructor injection of IHttpClientFactory and IOptions<ApiGatewayOptions>
        public OrderController(IHttpClientFactory httpClientFactory, IOptions<ApiGatewayOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _apiGatewayBaseUrl = options.Value.BaseUrl;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_orders);

        [HttpGet("{id}")]
        public ActionResult<Order> Get(int id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound("Order not found");
            return Ok(order);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<OrderDetails>> GetOrderDetails(int id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound("Order not found");

            var client = _httpClientFactory.CreateClient();

            // Use the configured API Gateway base URL to call ProductService
            var productServiceUrl = $"{_apiGatewayBaseUrl}/product/api/products/{order.ProductId}";

            var product = await client.GetFromJsonAsync<ProductDto>(productServiceUrl);

            if (product == null)
                return NotFound("Product not found");

            var orderDetails = new OrderDetails
            {
                Id = order.Id,
                Name = order.Name,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                CreatedAt = order.CreatedAt,
                ProductName = product.Name,
                ProductPrice = product.Price
            };

            return Ok(orderDetails);
        }

        [HttpPost]
        public IActionResult Create(OrderDto orderDto)
        {
            Order order = new Order
            {
                Id = _orders.Any() ? _orders.Max(o => o.Id) + 1 : 1,
                Name = orderDto.Name,
                ProductId = orderDto.ProductId,
                Quantity = orderDto.Quantity,
            };

            _orders.Add(order);
            return Ok(order);

            // Or to follow REST conventions:
            // return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }
    }

}
