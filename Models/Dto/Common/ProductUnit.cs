namespace Models.Dto.Common;

public class ProductUnit
{
    public long Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Category { get; set; }

    public long PriceCents { get; set; }

    public string PriceCurrency { get; set; }

    public string ProductUrl { get; set; }

    public string ImageUrl { get; set; }

    public int Stock { get; set; }

    public int DiscountPercent { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
