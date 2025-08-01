using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Models;
namespace OrderService.Repository
{
    public class OrderRepository(OrderDbContext context) : IOrderRepository
    {
        private readonly OrderDbContext _context = context;

        public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
        {
            return await _context.Orders
                .Select(o => new OrderResponseDto
                {
                    OrderId = o.Id,
                    OrderName = o.Name,
                    TotalAmount = o.TotalAmount,
                    OrderStaus = o.OrderStaus,
                    OrderDate = o.OrderDate,
                })
                .ToListAsync();

        }

        public async Task<Order?> GetAsync(int id)
        {
            return await _context.Orders.Include(o => o.OrderDetails)
                                        .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> CreateAsync(Order orderData)
        {
            _context.Orders.Add(orderData);
            await _context.SaveChangesAsync();
            return orderData;
        }

        public async Task<Order?> UpdateAsync(int id, OrderUpdateDto updateDto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return null;

            // Update fields
            order.Name = updateDto.OrderName;
            order.OrderStaus = updateDto.OrderStaus;

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _context.Orders
                                     .Include(o => o.OrderDetails)
                                     .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return false;

            // First remove related OrderDetails
            _context.OrderDetails.RemoveRange(order.OrderDetails);
            // Then remove the Order
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}