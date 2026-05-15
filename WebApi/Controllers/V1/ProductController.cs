using Microsoft.AspNetCore.Mvc;
using Models.Dto.V1.Requests;
using Models.Dto.V1.Responses;
using FluentValidation;

[Route("api/v1/products")]
[ApiController]
public class ProductController(
    ProductService productService,
    IValidatorFactory validatorFactory
) : ControllerBase
{
    [HttpPost("query")]
    public async Task<ActionResult<V1QueryProductsResponse>> QueryProducts(
        [FromBody] V1QueryProductsRequest request,
        CancellationToken token)
    {
        var validationResult = await validatorFactory.GetValidator<V1QueryProductsRequest>()
            .ValidateAsync(request, token);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var model = new QueryProductsDalModel
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Category = request.Category,
            MinPriceCents = request.MinPriceCents,
            MaxPriceCents = request.MaxPriceCents,
            InStock = request.InStock
        };

        var (products, total) = await productService.GetProducts(model, token);

        return Ok(new V1QueryProductsResponse
        {
            Products = products,
            Total = total,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Models.Dto.Common.ProductUnit>> GetProduct(
        long id,
        CancellationToken token)
    {
        try
        {
            var product = await productService.GetProductById(id, token);
            return Ok(product);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
