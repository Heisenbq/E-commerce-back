using Models.Dto.Common;

namespace Models.Dto.V1.Responses;

public class V1QueryProductsResponse
{
    public ProductUnit[] Products { get; set; }

    public long Total { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }
}
