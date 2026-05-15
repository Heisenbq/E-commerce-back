using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models.Dto.V1.Requests;
using Models.Dto.V1.Responses;
using FluentValidation;
using System.Security.Claims;

[Route("api/v1/cart")]
[ApiController]
[Authorize]
public class CartController(
    CartService cartService,
    IValidatorFactory validatorFactory
) : ControllerBase
{
    private long GetUserId()
    {
        var userIdClaim = User.FindFirst("userId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
            return userId;

        throw new UnauthorizedAccessException("User not authenticated");
    }

    [HttpGet]
    public async Task<ActionResult<V1GetCartResponse>> GetCart(CancellationToken token)
    {
        try
        {
            var userId = GetUserId();
            var cart = await cartService.GetCart(userId, token);

            return Ok(new V1GetCartResponse { Cart = cart });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPost("add")]
    public async Task<ActionResult<V1GetCartResponse>> AddToCart(
        [FromBody] V1AddToCartRequest request,
        CancellationToken token)
    {
        try
        {
            var validationResult = await validatorFactory.GetValidator<V1AddToCartRequest>()
                .ValidateAsync(request, token);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var userId = GetUserId();
            var cart = await cartService.AddToCart(userId, request.ProductId, request.Quantity, token);

            return Ok(new V1GetCartResponse { Cart = cart });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("items/{cartItemId}")]
    public async Task<ActionResult<V1GetCartResponse>> UpdateCartItem(
        long cartItemId,
        [FromBody] V1UpdateCartItemRequest request,
        CancellationToken token)
    {
        try
        {
            var validationResult = await validatorFactory.GetValidator<V1UpdateCartItemRequest>()
                .ValidateAsync(request, token);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var userId = GetUserId();
            var cart = await cartService.UpdateCartItem(userId, cartItemId, request.Quantity, token);

            return Ok(new V1GetCartResponse { Cart = cart });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete]
    public async Task<ActionResult<V1GetCartResponse>> RemoveFromCart(
        [FromBody] V1RemoveFromCartRequest request,
        CancellationToken token)
    {
        try
        {
            var userId = GetUserId();
            var cart = await cartService.RemoveFromCart(userId, request.CartItemIds, token);

            return Ok(new V1GetCartResponse { Cart = cart });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
}
