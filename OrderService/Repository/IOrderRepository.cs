
using OrderService.DTOs;
using OrderService.Models;

namespace OrderService.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderResponseDto>> GetAllAsync();
        Task<Order?> GetAsync(int id);
        Task<Order?> CreateAsync(Order orderData);
        Task<Order?> UpdateAsync(int id, OrderUpdateDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}