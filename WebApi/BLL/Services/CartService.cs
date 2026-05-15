using Models.Dto.Common;
using WebApi.DAL;

public class CartService(
    ICartRepository cartRepository,
    ICartItemRepository cartItemRepository,
    IProductRepository productRepository,
    UnitOfWork unitOfWork)
{
    public async Task<CartUnit> GetCart(long userId, CancellationToken token)
    {
        var cart = await cartRepository.GetOrCreateByUserId(userId, token);
        var items = await cartItemRepository.GetByCartId(cart.Id, token);

        var itemsWithProducts = new List<CartItemUnit>();
        foreach (var item in items)
        {
            var product = await productRepository.GetById(item.ProductId, token);
            itemsWithProducts.Add(new CartItemUnit
            {
                Id = item.Id,
                CartId = item.CartId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                Product = product != null ? MapProduct(product) : null
            });
        }

        var totalPrice = itemsWithProducts
            .Where(i => i.Product != null)
            .Sum(i => i.Product.PriceCents * i.Quantity);

        return new CartUnit
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = itemsWithProducts.ToArray(),
            TotalPriceCents = totalPrice,
            TotalPriceCurrency = "RUB",
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt
        };
    }

    public async Task<CartUnit> AddToCart(long userId, long productId, int quantity, CancellationToken token)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(token);

        try
        {
            var product = await productRepository.GetById(productId, token);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productId} not found");

            if (product.Stock < quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {product.Stock}, Requested: {quantity}");

            var cart = await cartRepository.GetOrCreateByUserId(userId, token);
            var now = DateTimeOffset.UtcNow;

            var cartItem = new V1CartItemDal
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity,
                CreatedAt = now,
                UpdatedAt = now
            };

            await cartItemRepository.AddItem(cartItem, token);
            await transaction.CommitAsync(token);

            return await GetCart(userId, token);
        }
        catch
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    public async Task<CartUnit> UpdateCartItem(long userId, long cartItemId, int quantity, CancellationToken token)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than 0");

        var cart = await cartRepository.GetOrCreateByUserId(userId, token);
        var item = await cartItemRepository.GetById(cartItemId, token);

        if (item == null || item.CartId != cart.Id)
            throw new InvalidOperationException("Cart item not found in your cart");

        await cartItemRepository.UpdateQuantity(cartItemId, quantity, token);
        return await GetCart(userId, token);
    }

    public async Task<CartUnit> RemoveFromCart(long userId, long[] cartItemIds, CancellationToken token)
    {
        var cart = await cartRepository.GetOrCreateByUserId(userId, token);
        var userItems = await cartItemRepository.GetByCartId(cart.Id, token);
        var userItemIds = userItems.Select(i => i.Id).ToHashSet();

        var validIds = cartItemIds.Where(id => userItemIds.Contains(id)).ToArray();
        if (validIds.Length > 0)
            await cartItemRepository.DeleteItems(validIds, token);

        return await GetCart(userId, token);
    }

    public async Task ClearCart(long userId, CancellationToken token)
    {
        var cart = await cartRepository.GetByUserId(userId, token);
        if (cart != null)
        {
            await cartItemRepository.DeleteByCartId(cart.Id, token);
        }
    }

    private ProductUnit MapProduct(V1ProductDal product)
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
