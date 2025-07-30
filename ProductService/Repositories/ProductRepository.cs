using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DTOs;
using ProductService.Models;

public class ProductRepository(ProductDbContext context) : IProductRepository
{
    private readonly ProductDbContext _context = context;

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        return await _context.Products
            .Select(p => new ProductDto
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Price = p.Price
            })
            .ToListAsync();

    }

    public async Task<ProductDto?> GetAsync(int id)
    {
        return await _context.Products
        .Where(p => p.Id == id)
        .Select(p => new ProductDto
        {
            ProductId = p.Id,
            ProductName = p.Name,
            Price = p.Price
        })
        .FirstOrDefaultAsync();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateAsync(int id, ProductInputDto inputDto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return null;
        }

        // Update fields
        product.Name = inputDto.ProductName;
        product.Price = inputDto.Price;

        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}