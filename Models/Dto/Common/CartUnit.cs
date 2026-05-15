namespace Models.Dto.Common;

public class CartUnit
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public CartItemUnit[] Items { get; set; }

    public long TotalPriceCents { get; set; }

    public string TotalPriceCurrency { get; set; } = "RUB";

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
