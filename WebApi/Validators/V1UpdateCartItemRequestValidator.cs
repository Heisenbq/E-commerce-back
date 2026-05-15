using FluentValidation;
using Models.Dto.V1.Requests;

public class V1UpdateCartItemRequestValidator : AbstractValidator<V1UpdateCartItemRequest>
{
    public V1UpdateCartItemRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000);
    }
}
