using MiniSalesOrderApp.Domain;

namespace MiniSalesOrderApp.Web.ViewModels.Orders;

public class OrderListItemViewModel
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemsCount { get; set; }
}
