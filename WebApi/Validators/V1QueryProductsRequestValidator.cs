using FluentValidation;
using Models.Dto.V1.Requests;

public class V1QueryProductsRequestValidator : AbstractValidator<V1QueryProductsRequest>
{
    public V1QueryProductsRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.MinPriceCents)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPriceCents.HasValue);

        RuleFor(x => x.MaxPriceCents)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPriceCents.HasValue);

        RuleFor(x => x)
            .Must(x => !x.MinPriceCents.HasValue || !x.MaxPriceCents.HasValue || x.MinPriceCents <= x.MaxPriceCents)
            .WithMessage("MinPriceCents must be less than or equal to MaxPriceCents");
    }
}
