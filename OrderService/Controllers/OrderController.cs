using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Controllers
{
    [ApiVersion("1.0")]
    public class OrderController(OrderDbContext dbContext, HttpClient httpClient, IOptions<ApiGatewayOptions> options) : BaseController
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _apiGatewayBaseUrl = options.Value.BaseUrl;
        private readonly OrderDbContext _dbContext = dbContext;

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var OrderDtos = await _dbContext.Orders
                .Select(o => new OrderResponseDto
                {
                    OrderId = o.Id,
                    OrderName = o.Name,
                    TotalAmount = o.TotalAmount,
                    OrderStaus = o.OrderStaus,
                    OrderDate = o.OrderDate,
                })
                .ToListAsync();

            // never returns null
            // It always returns a list object, even if there are zero items in the query
            // So OrderDtos is guaranteed to be a non-null list, possibly empty

            if (!OrderDtos.Any())
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "No Orders found!",
                });
            }

            return Ok(new Response<IEnumerable<OrderResponseDto>>
            {
                StatusCode = EResult.Success,
                Message = "Orders retrieved successfully",
                Data = OrderDtos
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _dbContext.Orders
                        .Include(o => o.OrderDetails)
                        .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Order not found!",
                });
            }

            var orderDetailDtos = new List<OrderDetailDto>();

            foreach (var detail in order.OrderDetails)
            {
                try
                {
                    var productServiceUrl = $"{_apiGatewayBaseUrl}/product/api/v1/product/{detail.ProductId}";

                    // Make the HTTP call and parse the wrapped response
                    var response = await _httpClient.GetFromJsonAsync<ApiResponse<ProductDto>>(productServiceUrl);

                    string productName = "Unknown";

                    if (response != null && response.StatusCode == 0 && response.Data != null)
                    {
                        productName = response.Data.ProductName ?? "Unnamed Product";
                    }

                    orderDetailDtos.Add(new OrderDetailDto
                    {

                        ProductId = detail.ProductId,
                        ProductName = productName,
                        Quantity = detail.Quantity,
                        Price = detail.Price
                    });
                }
                catch (HttpRequestException ex)
                {
                    return Ok(new Response
                    {
                        StatusCode = EResult.Error,
                        Message = $"Product service is unavailable: {ex.Message}"
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new Response
                    {
                        StatusCode = EResult.Error,
                        Message = $"An unexpected error occurred: {ex.Message}"
                    });
                }

            }
            var orderDto = new OrderDto
            {
                OrderId = order.Id,
                OrderName = order.Name,
                OrderDate = order.OrderDate,
                OrderStaus = order.OrderStaus,
                TotalAmount = order.TotalAmount,
                OrderDetails = orderDetailDtos
            };

            return Ok(new Response<OrderDto>
            {
                StatusCode = EResult.Success,
                Message = "Order retrieved successfully.",
                Data = orderDto
            });

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderInputDto orderInputDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            decimal totalAmount = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var detail in orderInputDto.OrderDetails)
            {
                try
                {
                    var productServiceUrl = $"{_apiGatewayBaseUrl}/product/api/v1/product/{detail.ProductId}";

                    // Make the HTTP call and parse the wrapped response
                    var response = await _httpClient.GetFromJsonAsync<ApiResponse<ProductDto>>(productServiceUrl);

                    // Check if product was not found or response is invalid
                    if (response == null || response.StatusCode != 0 || response.Data == null)
                    {
                        return Ok(new Response
                        {
                            StatusCode = EResult.Error,
                            Message = $"Product with ID {detail.ProductId} not found!"
                        });
                    }

                    var product = response.Data;

                    totalAmount += product.Price * detail.Quantity;

                    orderDetails.Add(new OrderDetail
                    {
                        ProductId = product.ProductId,
                        Quantity = detail.Quantity,
                        Price = product.Price
                    });
                }
                catch (HttpRequestException ex)
                {
                    return Ok(new Response
                    {
                        StatusCode = EResult.Error,
                        Message = $"Product service is unavailable: {ex.Message}"
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new Response
                    {
                        StatusCode = EResult.Error,
                        Message = $"An unexpected error occurred: {ex.Message}"
                    });
                }
            }

            var order = new Order
            {
                Name = orderInputDto.OrderName,
                OrderDate = DateTime.UtcNow,
                OrderStaus = 0,
                TotalAmount = totalAmount,
                OrderDetails = orderDetails
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            return Ok(new Response<int>
            {
                StatusCode = EResult.Success,
                Message = "Order created successfully.",
                Data = order.Id
            });

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderUpdateDto orderUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingOrder = await _dbContext.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Order not found!",
                });
            }

            existingOrder.Name = orderUpdateDto.OrderName;
            existingOrder.OrderStaus = orderUpdateDto.OrderStaus;

            // No need to call Update explicitly if tracked entity changed
            await _dbContext.SaveChangesAsync();

            return Ok(new Response<int>
            {
                StatusCode = EResult.Success,
                Message = "Order updated successfully.",
                Data = existingOrder.Id
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _dbContext.Orders
                                  .Include(o => o.OrderDetails)
                                  .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Order not found!",
                });
            }

            // First remove related OrderDetails
            _dbContext.OrderDetails.RemoveRange(order.OrderDetails);

            // Then remove the Order
            _dbContext.Orders.Remove(order);

            await _dbContext.SaveChangesAsync();

            return Ok(new Response
            {
                StatusCode = EResult.Success,
                Message = "Order deleted successfully!",
            });
        }

    }
}

