using FluentValidation;
using Models.Dto.V1.Requests;

public class V1AddToCartRequestValidator : AbstractValidator<V1AddToCartRequest>
{
    public V1AddToCartRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}
