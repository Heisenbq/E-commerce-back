using FluentValidation;
using Models.Dto.V1.Requests;

public class V1LoginRequestValidator : AbstractValidator<V1LoginRequest>
{
    public V1LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
