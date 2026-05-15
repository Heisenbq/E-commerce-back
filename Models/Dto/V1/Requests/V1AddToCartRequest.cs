namespace Models.Dto.V1.Requests;

public class V1AddToCartRequest
{
    public long ProductId { get; set; }

    public int Quantity { get; set; }
}
