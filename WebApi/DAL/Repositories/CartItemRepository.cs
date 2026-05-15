using Dapper;
using WebApi.DAL;

public class CartItemRepository(UnitOfWork unitOfWork) : ICartItemRepository
{
    public async Task<V1CartItemDal[]> GetByCartId(long cartId, CancellationToken token)
    {
        var sql = @"
            select 
                id,
                cart_id,
                product_id,
                quantity,
                created_at,
                updated_at
            from cart_items
            where cart_id = @CartId
        ";

        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryAsync<V1CartItemDal>(new CommandDefinition(
            sql, new { CartId = cartId }, cancellationToken: token));

        return res.ToArray();
    }

    public async Task<V1CartItemDal> GetById(long id, CancellationToken token)
    {
        var sql = @"
            select
                id,
                cart_id,
                product_id,
                quantity,
                created_at,
                updated_at
            from cart_items
            where id = @Id
        ";

        var conn = await unitOfWork.GetConnection(token);
        return await conn.QueryFirstOrDefaultAsync<V1CartItemDal>(new CommandDefinition(
            sql, new { Id = id }, cancellationToken: token));
    }

    public async Task<V1CartItemDal> AddItem(V1CartItemDal item, CancellationToken token)
    {
        var sql = @"
            insert into cart_items (cart_id, product_id, quantity, created_at, updated_at)
            values (@CartId, @ProductId, @Quantity, @CreatedAt, @UpdatedAt)
            returning id, cart_id, product_id, quantity, created_at, updated_at
        ";

        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryFirstAsync<V1CartItemDal>(new CommandDefinition(
            sql, item, cancellationToken: token));

        return res;
    }

    public async Task UpdateQuantity(long cartItemId, int quantity, CancellationToken token)
    {
        var sql = @"
            update cart_items
            set quantity = @Quantity, updated_at = @UpdatedAt
            where id = @Id
        ";

        var conn = await unitOfWork.GetConnection(token);
        await conn.ExecuteAsync(new CommandDefinition(
            sql, new { Id = cartItemId, Quantity = quantity, UpdatedAt = DateTimeOffset.UtcNow }, 
            cancellationToken: token));
    }

    public async Task DeleteItems(long[] cartItemIds, CancellationToken token)
    {
        if (cartItemIds.Length == 0) return;

        var sql = "delete from cart_items where id = ANY(@Ids)";

        var conn = await unitOfWork.GetConnection(token);
        await conn.ExecuteAsync(new CommandDefinition(
            sql, new { Ids = cartItemIds }, cancellationToken: token));
    }

    public async Task DeleteByCartId(long cartId, CancellationToken token)
    {
        var sql = "delete from cart_items where cart_id = @CartId";

        var conn = await unitOfWork.GetConnection(token);
        await conn.ExecuteAsync(new CommandDefinition(
            sql, new { CartId = cartId }, cancellationToken: token));
    }
}
