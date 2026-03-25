using Microsoft.EntityFrameworkCore;
using MiniSalesOrderApp.Domain;

namespace MiniSalesOrderApp.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!await context.Orders.AnyAsync())
        {
            var order1 = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = "Acme Corp",
                OrderDate = DateTime.UtcNow.AddDays(-2),
                Status = OrderStatus.Draft
            };

            var order1Items = new List<OrderItem>
            {
                CreateItem(order1.Id, "Laptop", 2, 1200.00m),
                CreateItem(order1.Id, "Mouse", 5, 25.50m)
            };

            order1.Items = order1Items;
            order1.TotalAmount = order1Items.Sum(x => x.TotalPrice);

            var order2 = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = "Northwind Traders",
                OrderDate = DateTime.UtcNow.AddDays(-1),
                Status = OrderStatus.Processing
            };

            var order2Items = new List<OrderItem>
            {
                CreateItem(order2.Id, "Monitor", 3, 310.00m),
                CreateItem(order2.Id, "Keyboard", 4, 75.25m),
                CreateItem(order2.Id, "USB-C Hub", 2, 49.90m)
            };

            order2.Items = order2Items;
            order2.TotalAmount = order2Items.Sum(x => x.TotalPrice);

            await context.Orders.AddRangeAsync(order1, order2);
            await context.SaveChangesAsync();
        }
    }

    private static OrderItem CreateItem(Guid orderId, string productName, int quantity, decimal unitPrice)
    {
        return new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice,
            TotalPrice = quantity * unitPrice
        };
    }
}
