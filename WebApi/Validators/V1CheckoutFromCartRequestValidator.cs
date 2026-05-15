using FluentValidation;
using Models.Dto.V1.Requests;

public class V1CheckoutFromCartRequestValidator : AbstractValidator<V1CheckoutFromCartRequest>
{
    public V1CheckoutFromCartRequestValidator()
    {
        RuleFor(x => x.DeliveryAddress)
            .NotEmpty()
            .MaximumLength(500);
    }
}
