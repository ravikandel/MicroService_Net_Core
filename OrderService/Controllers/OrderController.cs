using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OrderService.Common;
using OrderService.DTOs;
using OrderService.ExternalServices;
using OrderService.Logic;
using OrderService.Models;

namespace OrderService.Controllers
{
    [ApiVersion("1.0")]
    public class OrderController(IOrderLogic logic, IProductServiceClient productService) : BaseController
    {
        private readonly IOrderLogic _logic = logic;
        private readonly IProductServiceClient _productService = productService;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var orderResponseDto = await _logic.GetAllAsync();

            if (!orderResponseDto.Any())
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "No orders found!",
                });
            }

            return Ok(new Response<IEnumerable<OrderResponseDto>>
            {
                StatusCode = EResult.Success,
                Message = "Orders retrieved successfully!",
                Data = orderResponseDto
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _logic.GetAsync(id);
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
                var product = await _productService.GetProductAsync(detail.ProductId);

                var productName = product?.ProductName ?? "Unnamed Product";

                orderDetailDtos.Add(new OrderDetailDto
                {
                    ProductId = detail.ProductId,
                    ProductName = productName,
                    Quantity = detail.Quantity,
                    Price = detail.Price
                });
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
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Order information are required!",
                });
            }

            decimal totalAmount = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var detail in orderInputDto.OrderDetails)
            {
                ProductDto? product = null;
                try
                {
                    product = await _productService.GetProductAsync(detail.ProductId);

                    if (product == null)
                    {
                        return Ok(new Response
                        {
                            StatusCode = EResult.Error,
                            Message = $"Product with ID {detail.ProductId} not found!"
                        });
                    }

                    totalAmount += product.Price * detail.Quantity;

                    orderDetails.Add(new OrderDetail
                    {
                        ProductId = product.ProductId,
                        Quantity = detail.Quantity,
                        Price = product.Price
                    });
                }
                catch (HttpRequestException httpEx)
                {
                    return Ok(new Response
                    {
                        StatusCode = EResult.Error,
                        Message = $"[ProductService] HTTP error: {httpEx.Message}"
                    });
                }
                catch (NotSupportedException notSupportedEx)
                {
                    return Ok(new Response
                    {
                        StatusCode = EResult.Error,
                        Message = $"[ProductService] Response content type is not supported: {notSupportedEx.Message}"
                    });
                }
                catch (JsonException jsonEx)
                {
                    return Ok(new Response
                    {
                        StatusCode = EResult.Error,
                        Message = $"ProductService] JSON deserialization error: {jsonEx.Message}"
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new Response
                    {
                        StatusCode = EResult.Error,
                        Message = $"[ProductService] Unexpected error: {ex.Message}"
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

            var orderResponseDto = await _logic.CreateAsync(order);

            if (orderResponseDto == null)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Order cannot be created. Try Again!"
                });
            }

            return Ok(new Response<OrderResponseDto>
            {
                StatusCode = EResult.Success,
                Message = "Order created successfully!",
                Data = orderResponseDto
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderUpdateDto orderUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Order information are required!",
                });
            }
            var orderResponseDto = await _logic.UpdateAsync(id, orderUpdateDto);

            if (orderResponseDto == null)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Order not found!"
                });
            }

            return Ok(new Response<OrderResponseDto>
            {
                StatusCode = EResult.Success,
                Message = "Order updated successfully!",
                Data = orderResponseDto
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
                    Message = "Order not found!",
                });
            }

            return Ok(new Response
            {
                StatusCode = EResult.Success,
                Message = "Order deleted successfully!"
            });
        }

    }
}

