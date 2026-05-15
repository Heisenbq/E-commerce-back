using Models.Dto.Common;

public class ProductService(IProductRepository productRepository)
{
    public async Task<(ProductUnit[] products, long total)> GetProducts(
        QueryProductsDalModel model, 
        CancellationToken token)
    {
        var products = await productRepository.Query(model, token);
        var total = await productRepository.QueryTotal(model, token);

        var mapped = products.Select(Map).ToArray();
        
        return (mapped, total);
    }

    public async Task<ProductUnit> GetProductById(long id, CancellationToken token)
    {
        var product = await productRepository.GetById(id, token);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found");

        return Map(product);
    }

    public async Task<ProductUnit> CreateProduct(
        string title,
        string description,
        string category,
        long priceCents,
        string priceCurrency,
        string productUrl,
        int stock,
        int discountPercent,
        CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;
        var product = new V1ProductDal
        {
            Title = title,
            Description = description,
            Category = category,
            PriceCents = priceCents,
            PriceCurrency = priceCurrency,
            ProductUrl = productUrl,
            Stock = stock,
            DiscountPercent = discountPercent,
            CreatedAt = now,
            UpdatedAt = now
        };

        var created = await productRepository.BulkInsert(product, token);
        return Map(created);
    }

    private ProductUnit Map(V1ProductDal product)
    {
        return new ProductUnit
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Category = product.Category,
            PriceCents = product.PriceCents,
            PriceCurrency = product.PriceCurrency,
            ProductUrl = product.ProductUrl,
            Stock = product.Stock,
            DiscountPercent = product.DiscountPercent,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
