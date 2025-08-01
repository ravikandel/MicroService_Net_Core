using OrderService.DTOs;
using OrderService.Models;
using OrderService.Repository;

namespace OrderService.Logic
{
    public class OrderLogic(IOrderRepository repository) : IOrderLogic
    {
        private readonly IOrderRepository _repository = repository;

        public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
        {
            return await _repository.GetAllAsync();

        }

        public async Task<Order?> GetAsync(int id)
        {
            return await _repository.GetAsync(id);

        }

        public async Task<OrderResponseDto?> CreateAsync(Order orderData)
        {
            var order = await _repository.CreateAsync(orderData);

            if (order == null) return null;

            return new OrderResponseDto
            {
                OrderId = order.Id,
                OrderName = order.Name,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                OrderStaus = order.OrderStaus,
            };

        }

        public async Task<OrderResponseDto?> UpdateAsync(int id, OrderUpdateDto updateDto)
        {
            var order = await _repository.UpdateAsync(id, updateDto);

            if (order == null) return null;

            return new OrderResponseDto
            {
                OrderId = order.Id,
                OrderName = order.Name,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                OrderStaus = order.OrderStaus,
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}