using System.ComponentModel.DataAnnotations;

namespace MiniSalesOrderApp.Web.ViewModels.Orders;

public class AddProductViewModel
{
    [Required]
    public Guid OrderId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    [Display(Name = "Unit Price")]
    public decimal UnitPrice { get; set; }
}
