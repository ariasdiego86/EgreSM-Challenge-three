# Bonus Challenge - External Products Integration

## Overview
A new and fully isolated feature was added to consume an external API from the existing ASP.NET Core MVC monolith.

This implementation does **not** modify or mix with the existing Orders domain logic. Instead, it introduces a separate module for external products.

## What Was Implemented

### 1. HttpClient configuration
A named HttpClient was registered in `Program.cs`:

- Name: `ExternalProductsApi`
- Base address: `http://localhost:5005`

### 2. Isolated DTOs
New DTOs were created under a dedicated namespace:

- `MiniSalesOrderApp.Web.ViewModels.ExternalProducts.ProductDto`
- `MiniSalesOrderApp.Web.ViewModels.ExternalProducts.ProductPaginationResponseDto`

These map the external API JSON response for paginated products.

### 3. New independent controller
A new controller was added:

- `ExternalProductsController`

Main action:

- `Index(int pagina = 1, int cantidad = 20)`

Behavior:

- Uses `IHttpClientFactory` with the named client.
- Calls the external GET endpoint:
  - `/products/paged?pagina={pagina}&cantidad={cantidad}`
- Deserializes JSON using `System.Text.Json`.
- Returns a strongly-typed model to the Razor view.
- Handles basic error scenarios (API unavailable, timeout, invalid response, non-success HTTP code).

### 4. New independent Razor view
A dedicated view was added:

- `Views/ExternalProducts/Index.cshtml`

View features:

- Strongly typed to `ProductPaginationResponseDto`.
- Displays products in a Bootstrap table (`table table-striped table-bordered`).
- Includes pagination controls with Razor Tag Helpers.
- Shows previous/next buttons in disabled state when applicable.
- Displays a visual page indicator: `Page X of Y`.

### 5. Navigation entry in layout
A new menu item was added to the shared layout navbar:

- Label: `Productos Externos`
- Target: `ExternalProducts/Index`

## How To Access the Feature

### Option A: Through the top navigation menu
1. Run the MVC app.
2. In the main navbar, click **Productos Externos**.

### Option B: Direct URL
With the default launch settings, you can open:

- `http://localhost:5027/ExternalProducts`
- or `https://localhost:7156/ExternalProducts`

## Runtime Requirement
Make sure the external API is running at:

- `http://localhost:5005`

If the API is down or unreachable, the view will show a friendly warning message instead of breaking the page.
