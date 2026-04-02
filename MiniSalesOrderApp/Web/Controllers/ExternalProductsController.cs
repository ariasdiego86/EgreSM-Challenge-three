using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MiniSalesOrderApp.Web.ViewModels.ExternalProducts;

namespace MiniSalesOrderApp.Controllers;

public class ExternalProductsController : Controller
{
    private const string ExternalProductsApiClientName = "ExternalProductsApi";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IHttpClientFactory _httpClientFactory;

    public ExternalProductsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index(int pagina = 1, int cantidad = 20, CancellationToken cancellationToken = default)
    {
        pagina = pagina < 1 ? 1 : pagina;
        cantidad = cantidad is < 1 or > 100 ? 20 : cantidad;

        var fallbackModel = new ProductPaginationResponseDto(
            Pagina: pagina,
            CantidadSolicitada: cantidad,
            TotalRegistros: 0,
            TotalPaginas: 1,
            Data: []);

        try
        {
            var client = _httpClientFactory.CreateClient(ExternalProductsApiClientName);
            using var response = await client.GetAsync($"/products/paged?pagina={pagina}&cantidad={cantidad}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                ViewData["ExternalProductsError"] = $"No se pudo obtener la lista de productos externos. Estado HTTP: {(int)response.StatusCode}.";
                return View(fallbackModel);
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var model = await JsonSerializer.DeserializeAsync<ProductPaginationResponseDto>(responseStream, JsonOptions, cancellationToken);

            if (model is null)
            {
                ViewData["ExternalProductsError"] = "La API externa devolvio una respuesta vacia o invalida.";
                return View(fallbackModel);
            }

            return View(model);
        }
        catch (HttpRequestException)
        {
            ViewData["ExternalProductsError"] = "No fue posible conectar con la API externa en http://localhost:5005.";
            return View(fallbackModel);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            ViewData["ExternalProductsError"] = "La solicitud a la API externa excedio el tiempo de espera.";
            return View(fallbackModel);
        }
    }
}
