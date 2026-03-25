namespace MiniSalesOrderApp.Domain;

public class Order
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public decimal TotalAmount { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
