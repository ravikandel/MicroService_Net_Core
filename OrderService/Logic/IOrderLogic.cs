using OrderService.DTOs;
using OrderService.Models;

namespace OrderService.Logic
{
    public interface IOrderLogic
    {
        Task<IEnumerable<OrderResponseDto>> GetAllAsync();
        Task<Order?> GetAsync(int id);
        Task<OrderResponseDto?> CreateAsync(Order orderData);
        Task<OrderResponseDto?> UpdateAsync(int id, OrderUpdateDto updateDto);
        Task<bool> DeleteAsync(int id);

    }
}