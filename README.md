# Mini Sales Order App

## General Description
Mini Sales Order App is an ASP.NET Core MVC web application to manage customer sales orders.
It allows you to:
- Create orders.
- Add and remove products per order.
- Calculate totals automatically.
- Move the order status through stages.
- View, filter, and paginate orders.

## Technologies Used
- .NET 10 (ASP.NET Core MVC)
- Razor Views (.cshtml)
- Bootstrap 5
- Entity Framework Core 10
- SQL Server
- SQL Server Management Studio (for visual administration)
- Dependency Injection
- Repository Pattern
- SOLID Principles

## Project Architecture
A single web project is used, organized by folders following a Clean Architecture style.

Layers and responsibilities:
- Domain: entities, enums, and repository interfaces.
- Application: business services and domain rules.
- Infrastructure: EF Core, DbContext, Fluent API configurations, seeder, and repository.
- Web: presentation layer (controllers and view models).
- Views: Razor views for the server-side UI.

Main structure:
- Domain
- Application
- Infrastructure
- Web
- Views

## Execution Ports
According to launch settings:
- HTTP: http://localhost:5027
- HTTPS: https://localhost:7156

If the default port is already in use, you can run on a different port like this:
- dotnet run --project c:\egresm\challenge-three\MiniSalesOrderApp\MiniSalesOrderApp.csproj --urls http://localhost:5099

## Database
Database name:
- ChallengeThree

Currently configured server:
- .\SQLEXPRESS

Connection string in appsettings:
- Server=.\SQLEXPRESS;Database=ChallengeThree;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True

Configuration file:
- appsettings.json

## Automatic Seed on Startup
When the app starts, Program runs:
1. EnsureCreatedAsync to create the database/tables if they do not exist.
2. SeedAsync for initial data.

Seeder behavior:
- Inserts data only if Orders is empty.
- Creates 2 sample orders (Draft and Processing).
- Creates 2 or 3 items per order.
- Calculates TotalPrice per item and TotalAmount per order.

Key files:
- Program.cs
- Infrastructure/DatabaseSeeder.cs

## Database Schema
### Orders Table
- Id (Guid, PK)
- CustomerName (nvarchar(100), required)
- OrderDate (datetime, required)
- Status (int, enum OrderStatus)
- TotalAmount (decimal(18,2), required)

### OrderItems Table
- Id (Guid, PK)
- OrderId (Guid, FK -> Orders.Id)
- ProductName (nvarchar(150), required)
- Quantity (int, required, check > 0)
- UnitPrice (decimal(18,2), required)
- TotalPrice (decimal(18,2), required)

Relationship:
- 1:N from Orders to OrderItems
- Cascade delete configured

## Functional Flow
1. Create an order with a customer name.
2. Add products to the order.
3. Recalculate totals automatically when adding or removing products.
4. Change status sequentially:
   - Draft -> Processing -> Shipped -> Invoiced
5. When an order is Invoiced:
   - It does not allow modifications (edit customer, add/remove products, delete order).

## How to Run the Project
Prerequisites:
- .NET 10 SDK
- SQL Server Express (SQLEXPRESS)

Steps:
1. Go to the project folder:
   - cd c:\egresm\challenge-three\MiniSalesOrderApp
2. Restore packages (optional):
   - dotnet restore
3. Run the application:
   - dotnet run
4. Open in your browser:
   - https://localhost:7156/Orders
   - or http://localhost:5027/Orders

## How to View the Database in SQL Server Management Studio
1. Open SSMS.
2. Connect to:
   - Server Name: .\SQLEXPRESS
   - Authentication: Windows Authentication
3. Expand Databases and refresh.
4. Verify that ChallengeThree exists.
5. Expand tables:
   - Orders
   - OrderItems

## Key Project Files
- Program.cs
- appsettings.json
- Domain/Order.cs
- Domain/OrderItem.cs
- Domain/OrderStatus.cs
- Domain/IOrderRepository.cs
- Application/IOrderService.cs
- Application/OrderService.cs
- Infrastructure/AppDbContext.cs
- Infrastructure/DatabaseSeeder.cs
- Infrastructure/OrderRepository.cs
- Infrastructure/Configurations/OrderConfiguration.cs
- Infrastructure/Configurations/OrderItemConfiguration.cs
- Web/Controllers/OrdersController.cs
- Views/Orders/Index.cshtml
- Views/Orders/Details.cshtml
- Views/Orders/Create.cshtml
- Views/Orders/AddProduct.cshtml
- Views/Orders/Edit.cshtml

## Operational Notes
- If you get a port-in-use error, run with --urls on another port.
- If you get a file-lock error during build, stop the process currently running the app and build again.
- If you want to reseed, clear the Orders table and restart the app (the seeder inserts again when Orders is empty).
