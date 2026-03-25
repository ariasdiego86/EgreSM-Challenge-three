# Contexto del Proyecto
Actúa como un Arquitecto de Software .NET Senior. Vamos a desarrollar una aplicación web llamada "Mini Sales Order App". 
El objetivo es simular un proceso real de gestión de órdenes de venta, permitiendo a los usuarios crear órdenes de clientes, agregar productos, calcular totales y mover cada orden a través de etapas clave (Procesando, Enviado, Facturado).

# Stack Tecnológico y Reglas Estrictas
* **Framework:** .NET 10 SDK
* **Arquitectura:** Un solo proyecto **ASP.NET Core MVC (Web)**. **NO** crees múltiples proyectos (.csproj). Usa carpetas para separar las capas siguiendo los principios de Clean Architecture.
* **Frontend:** Vistas Razor (.cshtml) renderizadas desde el servidor usando ASP.NET Core MVC. Usa Bootstrap 5 para el diseño. Nada de frameworks JS, ni APIs REST separadas.
* **Base de Datos:** SQL Server (LocalDB)
* **ORM:** Entity Framework Core
* **Patrones:** Patrón Repository, Inyección de Dependencias, Principios SOLID.

# Estructura de Carpetas (Clean Architecture)
El proyecto debe contener estas carpetas principales: `Domain`, `Application`, `Infrastructure`, `Web` (esta última contendrá `Controllers` y `Views`).

# Paso 1: Domain (Entidades y Esquema)
Crea las siguientes entidades con sus tipos de datos en C# dentro de la carpeta `Domain`:
* `OrderStatus` (Enum): `Draft` (0), `Processing` (1), `Shipped` (2), `Invoiced` (3).
* `Order` (Entidad): 
  - `Id` (Guid)
  - `CustomerName` (string, max 100 caracteres, requerido)
  - `OrderDate` (DateTime, por defecto: DateTime.UtcNow)
  - `Status` (OrderStatus, por defecto: Draft)
  - `TotalAmount` (decimal, precisión 18,2)
  - Navegación: `ICollection<OrderItem> Items`
* `OrderItem` (Entidad): 
  - `Id` (Guid)
  - `OrderId` (Guid, FK a Order)
  - `ProductName` (string, max 150 caracteres, requerido)
  - `Quantity` (int, mayor a 0)
  - `UnitPrice` (decimal, precisión 18,2)
  - `TotalPrice` (decimal, precisión 18,2, debe ser Quantity * UnitPrice)

# Paso 2: Infrastructure (EF Core y Configuración de DB)
En la carpeta `Infrastructure`, **NO** uses Data Annotations en las entidades. Usa `IEntityTypeConfiguration<T>` (Fluent API) para configurar tablas, claves y tipos decimales (`decimal(18,2)`). Configura la relación 1:N entre Order y OrderItem.

Crea la clase `AppDbContext` inyectando `DbContextOptions<AppDbContext>`. Aplica las configuraciones en `OnModelCreating`.

**IMPORTANTE:** Genera el código para actualizar el archivo `appsettings.json` con la siguiente cadena de conexión exacta:
`"Server=(localdb)\\mssqllocaldb;Database=ChallengeThree;Trusted_Connection=True;MultipleActiveResultSets=true"`
Llámala "DefaultConnection".

# Paso 3: Infrastructure (Database Seeder)
En la carpeta `Infrastructure`, crea una clase `DatabaseSeeder` estática con un método `public static async Task SeedAsync(AppDbContext context)`.
* Debe verificar si `!context.Orders.Any()`. Si está vacía, debe insertar 2 Órdenes de prueba (una en `Draft`, otra en `Processing`) y asociarles 2 o 3 `OrderItems` a cada una calculando bien su `TotalPrice` y el `TotalAmount` de la orden.

# Paso 4: Domain e Infrastructure (Repositorio)
* En `Domain`: Define la interfaz `IOrderRepository` (métodos asíncronos para CRUD).
* En `Infrastructure`: Implementa `OrderRepository` inyectando `AppDbContext`.

# Paso 5: Application (Servicios)
* En `Application`: Define `IOrderService` y su implementación `OrderService`.
* `OrderService` debe inyectar `IOrderRepository`. Debe contener la lógica de negocio pura:
  - Al agregar o eliminar un `OrderItem`, debe recalcular automáticamente el `TotalAmount` de la `Order`.
  - Método `UpdateOrderStatusAsync`: Debe validar que las transiciones sean lógicas (ej. no saltar de `Draft` a `Invoiced` directamente).

# Instrucción estricta de Ejecución para la IA
Por favor, confírmame que has entendido todos los requerimientos técnicos, la arquitectura solicitada y la configuración de la base de datos.
A continuación, **GENERA ÚNICAMENTE EL CÓDIGO DE LOS PASOS 1, 2 Y 3** (Dominio, DbContext, Fluent API, Seeder y el cambio en appsettings.json). 
NO generes repositorios, servicios, controladores ni vistas todavía para no saturar el límite de caracteres. Espera mi indicación para continuar con los pasos 4 y 5.