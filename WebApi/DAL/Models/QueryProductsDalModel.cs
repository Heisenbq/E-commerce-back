public class QueryProductsDalModel
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string Category { get; set; }

    public long? MinPriceCents { get; set; }

    public long? MaxPriceCents { get; set; }

    public bool? InStock { get; set; }
}
