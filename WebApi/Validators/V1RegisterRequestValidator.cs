using FluentValidation;
using Models.Dto.V1.Requests;

public class V1RegisterRequestValidator : AbstractValidator<V1RegisterRequest>
{
    public V1RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(100);
    }
}
