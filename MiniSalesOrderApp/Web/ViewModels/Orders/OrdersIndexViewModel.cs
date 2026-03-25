using MiniSalesOrderApp.Domain;

namespace MiniSalesOrderApp.Web.ViewModels.Orders;

public class OrdersIndexViewModel
{
    public IReadOnlyList<OrderListItemViewModel> Orders { get; set; } = [];
    public string? SearchCustomer { get; set; }
    public OrderStatus? StatusFilter { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => TotalItems == 0 ? 1 : (int)Math.Ceiling((double)TotalItems / PageSize);
}
