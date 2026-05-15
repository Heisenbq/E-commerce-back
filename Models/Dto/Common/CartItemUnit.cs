namespace Models.Dto.Common;

public class CartItemUnit
{
    public long Id { get; set; }

    public long CartId { get; set; }

    public long ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    // Для удобства при возврате с фронтенда
    public ProductUnit Product { get; set; }
}
