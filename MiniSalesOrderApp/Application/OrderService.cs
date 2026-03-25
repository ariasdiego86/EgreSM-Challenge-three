using MiniSalesOrderApp.Domain;

namespace MiniSalesOrderApp.Application;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<Order>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await _orderRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _orderRepository.GetByIdAsync(orderId, cancellationToken);
    }

    public async Task<Order> CreateOrderAsync(string customerName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("Customer name is required.", nameof(customerName));
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = customerName.Trim(),
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Draft,
            TotalAmount = 0m
        };

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return order;
    }

    public async Task UpdateCustomerNameAsync(Guid orderId, string customerName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("Customer name is required.", nameof(customerName));
        }

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order with id {orderId} was not found.");

        EnsureOrderIsEditable(order);

        order.CustomerName = customerName.Trim();
        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task AddOrderItemAsync(Guid orderId, string productName, int quantity, decimal unitPrice, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("Product name is required.", nameof(productName));
        }

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order with id {orderId} was not found.");

        EnsureOrderIsEditable(order);

        var item = new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            ProductName = productName.Trim(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            TotalPrice = quantity * unitPrice
        };

        await _orderRepository.AddItemAsync(item, cancellationToken);

        // Recalculate using distinct item ids to prevent transient in-memory duplicates.
        order.TotalAmount = order.Items
            .Concat(new[] { item })
            .GroupBy(x => x.Id)
            .Sum(x => x.First().TotalPrice);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveOrderItemAsync(Guid orderId, Guid orderItemId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order with id {orderId} was not found.");

        EnsureOrderIsEditable(order);

        var item = order.Items.FirstOrDefault(x => x.Id == orderItemId)
            ?? throw new KeyNotFoundException($"Order item with id {orderItemId} was not found in order {orderId}.");

        await _orderRepository.RemoveItemAsync(orderItemId, cancellationToken);

        // Recalculate using the resulting set (excluding removed item id) instead of relying on reference removal.
        order.TotalAmount = order.Items
            .Where(x => x.Id != orderItemId)
            .Sum(x => x.TotalPrice);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order with id {orderId} was not found.");

        var currentStatus = order.Status;

        if (newStatus == currentStatus)
        {
            return;
        }

        if ((int)newStatus != (int)currentStatus + 1)
        {
            throw new InvalidOperationException(
                $"Invalid status transition from {currentStatus} to {newStatus}. Allowed transition is only to the immediate next status.");
        }

        order.Status = newStatus;
        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order with id {orderId} was not found.");

        EnsureOrderIsEditable(order);

        await _orderRepository.DeleteAsync(orderId, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureOrderIsEditable(Order order)
    {
        if (order.Status == OrderStatus.Invoiced)
        {
            throw new InvalidOperationException("Invoiced orders cannot be modified.");
        }
    }
}
