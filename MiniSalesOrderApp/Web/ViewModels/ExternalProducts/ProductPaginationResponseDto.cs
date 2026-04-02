namespace MiniSalesOrderApp.Web.ViewModels.ExternalProducts;

public record ProductPaginationResponseDto(
    int Pagina,
    int CantidadSolicitada,
    int TotalRegistros,
    int TotalPaginas,
    List<ProductDto> Data);
