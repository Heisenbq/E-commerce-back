using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using Models.Dto.Common;
using Models.Dto.V1.Requests;
using Models.Dto.V1.Responses;
using FluentValidation;
using Oms.Services;
using Messages;

[Route("api/v1/order")]
public class OrderController(
    OrderService orderService,
    IValidatorFactory validatorFactory,
    RabbitMqService rabbitMqService,
    CartService cartService
) : ControllerBase
{
    private long GetUserId()
    {
        var userIdClaim = User.FindFirst("userId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
            return userId;

        throw new UnauthorizedAccessException("User not authenticated");
    }

    [HttpPost("batch-create")]
    public async Task<ActionResult<V1CreateOrderResponse>> V1BatchCreate([FromBody] V1CreateOrderRequest request, CancellationToken token)
    {
        var validationResult = await validatorFactory.GetValidator<V1CreateOrderRequest>()
            .ValidateAsync(request, token);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var res = await orderService.BatchInsert(request.Orders.Select(x => new OrderUnit
        {
            CustomerId = x.CustomerId,
            DeliveryAddress = x.DeliveryAddress,
            TotalPriceCents = x.TotalPriceCents,
            TotalPriceCurrency = x.TotalPriceCurrency,
            OrderStatus = "created",
            OrderItems = x.OrderItems.Select(p => new OrderItemUnit
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                ProductTitle = p.ProductTitle,
                ProductUrl = p.ProductUrl,
                PriceCents = p.PriceCents,
                PriceCurrency = p.PriceCurrency,
            }).ToArray()
        }).ToArray(), token);

        var messages = res.Select(x => new OmsOrderCreatedMessage
        {
            Id = x.Id,
            CustomerId = x.CustomerId,
            DeliveryAddress = x.DeliveryAddress,
            TotalPriceCents = x.TotalPriceCents,
            TotalPriceCurrency = x.TotalPriceCurrency,
            OrderStatus = x.OrderStatus,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            OrderItems = x.OrderItems.Select(p => new OrderItemMessage
            {
                Id = p.Id,
                OrderId = p.OrderId,
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                ProductTitle = p.ProductTitle,
                ProductUrl = p.ProductUrl,
                PriceCents = p.PriceCents,
                PriceCurrency = p.PriceCurrency,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToArray()
        });

        await rabbitMqService.Publish(messages, token);

        return Ok(new V1CreateOrderResponse
        {
            Orders = Map(res)
        });
    }

    [HttpPost("query")]
    public async Task<ActionResult<V1QueryOrdersResponse>> V1QueryOrders([FromBody] V1QueryOrdersRequest request, CancellationToken token)
    {
        var validationResult = await validatorFactory.GetValidator<V1QueryOrdersRequest>()
            .ValidateAsync(request, token);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var res = await orderService.GetOrders(new QueryOrderItemsModel
        {
            Ids = request.Ids,
            CustomerIds = request.CustomerIds,
            Page = request.Page,
            PageSize = request.PageSize,
            IncludeOrderItems = request.IncludeOrderItems
        }, token);
            
        return Ok(new V1QueryOrdersResponse
        {
            Orders = Map(res)
        });
    }

    [HttpPost("update-status")]
    public async Task<ActionResult<V1UpdateOrderStatusResponse>> V1UpdateOrdersStatus(
        [FromBody] V1UpdateOrdersStatusRequest request,
        CancellationToken token)
    {
        var validationResult = await validatorFactory.GetValidator<V1UpdateOrdersStatusRequest>()
            .ValidateAsync(request, token);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        try
        {
            await orderService.UpdateStatus(request.OrderIds, request.NewStatus, token);

            var normalizedStatus = request.NewStatus?.Trim().ToLowerInvariant() ?? string.Empty;
            var messages = request.OrderIds.Select(id => new OmsOrderStatusChangedMessage
            {
                OrderId = id,
                OrderStatus = normalizedStatus,
                UpdatedAt = DateTimeOffset.UtcNow
            });

            await rabbitMqService.Publish(messages, token);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        return Ok(new V1UpdateOrderStatusResponse());
    }

    [HttpPost("checkout-from-cart")]
    [Authorize]
    public async Task<ActionResult<V1CreateOrderResponse>> CheckoutFromCart(
        [FromBody] V1CheckoutFromCartRequest request,
        CancellationToken token)
    {
        try
        {
            var validationResult = await validatorFactory.GetValidator<V1CheckoutFromCartRequest>()
                .ValidateAsync(request, token);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var userId = GetUserId();
            var cart = await cartService.GetCart(userId, token);

            if (cart.Items == null || cart.Items.Length == 0)
            {
                return BadRequest(new { error = "Cart is empty" });
            }

            // Create order from cart items
            var orderItems = cart.Items.Select(item => new OrderItemUnit
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ProductTitle = item.Product?.Title ?? $"Product {item.ProductId}",
                ProductUrl = item.Product?.ProductUrl ?? string.Empty,
                PriceCents = item.Product?.PriceCents ?? 0,
                PriceCurrency = item.Product?.PriceCurrency ?? "RUB"
            }).ToArray();

            var orderUnit = new OrderUnit
            {
                CustomerId = userId,
                DeliveryAddress = request.DeliveryAddress,
                TotalPriceCents = cart.TotalPriceCents,
                TotalPriceCurrency = cart.TotalPriceCurrency,
                OrderStatus = "created",
                OrderItems = orderItems
            };

            var res = await orderService.BatchInsert(new[] { orderUnit }, token);

            var messages = res.Select(x => new OmsOrderCreatedMessage
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                DeliveryAddress = x.DeliveryAddress,
                TotalPriceCents = x.TotalPriceCents,
                TotalPriceCurrency = x.TotalPriceCurrency,
                OrderStatus = x.OrderStatus,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                OrderItems = x.OrderItems.Select(p => new OrderItemMessage
                {
                    Id = p.Id,
                    OrderId = p.OrderId,
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    ProductTitle = p.ProductTitle,
                    ProductUrl = p.ProductUrl,
                    PriceCents = p.PriceCents,
                    PriceCurrency = p.PriceCurrency,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToArray()
            });

            await rabbitMqService.Publish(messages, token);

            await cartService.ClearCart(userId, token);

            return Ok(new V1CreateOrderResponse
            {
                Orders = Map(res)
            });
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

    private Models.Dto.Common.OrderUnit[] Map(OrderUnit[] orders)
    {
        return orders.Select(x => new Models.Dto.Common.OrderUnit
        {
            Id = x.Id,
            CustomerId = x.CustomerId,
            DeliveryAddress = x.DeliveryAddress,
            TotalPriceCents = x.TotalPriceCents,
            TotalPriceCurrency = x.TotalPriceCurrency,
            OrderStatus = x.OrderStatus,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            OrderItems = x.OrderItems.Select(p => new Models.Dto.Common.OrderItemUnit
            {
                Id = p.Id,
                OrderId = p.OrderId,
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                ProductTitle = p.ProductTitle,
                ProductUrl = p.ProductUrl,
                PriceCents = p.PriceCents,
                PriceCurrency = p.PriceCurrency,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToArray()
        }).ToArray();
    }
}
