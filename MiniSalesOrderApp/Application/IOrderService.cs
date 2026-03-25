using MiniSalesOrderApp.Domain;

namespace MiniSalesOrderApp.Application;

public interface IOrderService
{
    Task<IReadOnlyList<Order>> GetOrdersAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Order> CreateOrderAsync(string customerName, CancellationToken cancellationToken = default);
    Task UpdateCustomerNameAsync(Guid orderId, string customerName, CancellationToken cancellationToken = default);
    Task AddOrderItemAsync(Guid orderId, string productName, int quantity, decimal unitPrice, CancellationToken cancellationToken = default);
    Task RemoveOrderItemAsync(Guid orderId, Guid orderItemId, CancellationToken cancellationToken = default);
    Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken = default);
    Task DeleteOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
}
