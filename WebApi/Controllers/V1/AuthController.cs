using Microsoft.AspNetCore.Mvc;
using Models.Dto.V1.Requests;
using Models.Dto.V1.Responses;
using FluentValidation;

[Route("api/v1/auth")]
[ApiController]
public class AuthController(
    AuthService authService,
    IValidatorFactory validatorFactory
) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<V1AuthResponse>> Register(
        [FromBody] V1RegisterRequest request,
        CancellationToken token)
    {
        try
        {
            var validationResult = await validatorFactory.GetValidator<V1RegisterRequest>()
                .ValidateAsync(request, token);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var (user, jwtToken) = await authService.Register(request.Username, request.Email, request.Password, token);

            return Ok(new V1AuthResponse
            {
                User = user,
                Token = jwtToken
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<V1AuthResponse>> Login(
        [FromBody] V1LoginRequest request,
        CancellationToken token)
    {
        try
        {
            var validationResult = await validatorFactory.GetValidator<V1LoginRequest>()
                .ValidateAsync(request, token);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var (user, jwtToken) = await authService.Login(request.Username, request.Password, token);

            return Ok(new V1AuthResponse
            {
                User = user,
                Token = jwtToken
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
