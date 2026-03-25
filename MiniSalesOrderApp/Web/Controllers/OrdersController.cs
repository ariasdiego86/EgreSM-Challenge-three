using Microsoft.AspNetCore.Mvc;
using MiniSalesOrderApp.Application;
using MiniSalesOrderApp.Domain;
using MiniSalesOrderApp.Web.ViewModels.Orders;

namespace MiniSalesOrderApp.Controllers;

public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index(string? searchCustomer, OrderStatus? statusFilter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 5 or > 50 ? 10 : pageSize;

        var orders = await _orderService.GetOrdersAsync(cancellationToken);
        var query = orders.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchCustomer))
        {
            query = query.Where(x => x.CustomerName.Contains(searchCustomer, StringComparison.OrdinalIgnoreCase));
        }

        if (statusFilter.HasValue)
        {
            query = query.Where(x => x.Status == statusFilter.Value);
        }

        var totalItems = query.Count();

        var pagedOrders = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new OrderListItemViewModel
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                OrderDate = x.OrderDate,
                Status = x.Status,
                TotalAmount = x.TotalAmount,
                ItemsCount = x.Items.Count
            })
            .ToList();

        var model = new OrdersIndexViewModel
        {
            Orders = pagedOrders,
            SearchCustomer = searchCustomer,
            StatusFilter = statusFilter,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return View(model);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        return View(MapToDetails(order));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateOrderViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        if (order.Status == OrderStatus.Invoiced)
        {
            TempData["Error"] = "Invoiced orders cannot be modified.";
            return RedirectToAction(nameof(Details), new { id = order.Id });
        }

        var model = new EditOrderViewModel
        {
            Id = order.Id,
            CustomerName = order.CustomerName
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditOrderViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _orderService.UpdateCustomerNameAsync(model.Id, model.CustomerName, cancellationToken);
            TempData["Success"] = "Order updated successfully.";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        catch (Exception ex) when (ex is KeyNotFoundException or ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOrderViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var order = await _orderService.CreateOrderAsync(model.CustomerName, cancellationToken);
        TempData["Success"] = "Order created successfully.";

        return RedirectToAction(nameof(Details), new { id = order.Id });
    }

    [HttpGet]
    public async Task<IActionResult> AddProduct(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        if (order.Status == OrderStatus.Invoiced)
        {
            TempData["Error"] = "Invoiced orders cannot be modified.";
            return RedirectToAction(nameof(Details), new { id = order.Id });
        }

        var model = new AddProductViewModel
        {
            OrderId = order.Id,
            CustomerName = order.CustomerName,
            Quantity = 1
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProduct(AddProductViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _orderService.AddOrderItemAsync(model.OrderId, model.ProductName, model.Quantity, model.UnitPrice, cancellationToken);
            TempData["Success"] = "Product added successfully.";
            return RedirectToAction(nameof(Details), new { id = model.OrderId });
        }
        catch (Exception ex) when (ex is KeyNotFoundException or ArgumentException or ArgumentOutOfRangeException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveProduct(Guid orderId, Guid orderItemId, CancellationToken cancellationToken)
    {
        try
        {
            await _orderService.RemoveOrderItemAsync(orderId, orderItemId, cancellationToken);
            TempData["Success"] = "Product removed successfully.";
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken)
    {
        try
        {
            await _orderService.UpdateOrderStatusAsync(orderId, newStatus, cancellationToken);
            TempData["Success"] = "Order status updated successfully.";
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _orderService.DeleteOrderAsync(id, cancellationToken);
            TempData["Success"] = "Order deleted successfully.";
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private static OrderDetailsViewModel MapToDetails(Order order)
    {
        return new OrderDetailsViewModel
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            NextAllowedStatus = GetNextAllowedStatus(order.Status),
            Items = order.Items.Select(x => new OrderItemViewModel
            {
                Id = x.Id,
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                TotalPrice = x.TotalPrice
            }).ToList()
        };
    }

    private static OrderStatus? GetNextAllowedStatus(OrderStatus currentStatus)
    {
        return currentStatus switch
        {
            OrderStatus.Draft => OrderStatus.Processing,
            OrderStatus.Processing => OrderStatus.Shipped,
            OrderStatus.Shipped => OrderStatus.Invoiced,
            _ => null
        };
    }
}
