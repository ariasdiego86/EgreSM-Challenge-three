using Microsoft.EntityFrameworkCore;
using MiniSalesOrderApp.Domain;

namespace MiniSalesOrderApp.Infrastructure;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(x => x.Items)
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Update(order);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (order is not null)
        {
            _context.Orders.Remove(order);
        }
    }

    public async Task AddItemAsync(OrderItem item, CancellationToken cancellationToken = default)
    {
        await _context.OrderItems.AddAsync(item, cancellationToken);
    }

    public async Task RemoveItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = await _context.OrderItems.FirstOrDefaultAsync(x => x.Id == itemId, cancellationToken);
        if (item is not null)
        {
            _context.OrderItems.Remove(item);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
