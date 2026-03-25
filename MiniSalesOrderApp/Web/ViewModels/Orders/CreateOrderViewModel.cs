using System.ComponentModel.DataAnnotations;

namespace MiniSalesOrderApp.Web.ViewModels.Orders;

public class CreateOrderViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Customer Name")]
    public string CustomerName { get; set; } = string.Empty;
}
