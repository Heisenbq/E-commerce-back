using Dapper;
using WebApi.DAL;

public class CartRepository(UnitOfWork unitOfWork) : ICartRepository
{
    public async Task<V1CartDal> GetByUserId(long userId, CancellationToken token)
    {
        var sql = @"
            select 
                id,
                user_id,
                created_at,
                updated_at
            from carts
            where user_id = @UserId
        ";

        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryFirstOrDefaultAsync<V1CartDal>(new CommandDefinition(
            sql, new { UserId = userId }, cancellationToken: token));

        return res;
    }

    public async Task<V1CartDal> GetOrCreateByUserId(long userId, CancellationToken token)
    {
        var sql = @"
            WITH inserted AS (
                INSERT INTO carts (user_id, created_at, updated_at)
                VALUES (@UserId, @CreatedAt, @UpdatedAt)
                ON CONFLICT (user_id) DO NOTHING
                RETURNING id, user_id, created_at, updated_at
            )
            SELECT * FROM inserted
            UNION ALL
            SELECT id, user_id, created_at, updated_at FROM carts WHERE user_id = @UserId
            LIMIT 1
        ";

        var now = DateTimeOffset.UtcNow;
        var conn = await unitOfWork.GetConnection(token);
        return await conn.QueryFirstAsync<V1CartDal>(new CommandDefinition(
            sql, new { UserId = userId, CreatedAt = now, UpdatedAt = now }, cancellationToken: token));
    }

    public async Task<V1CartDal> Create(V1CartDal cart, CancellationToken token)
    {
        var sql = @"
            insert into carts (user_id, created_at, updated_at)
            values (@UserId, @CreatedAt, @UpdatedAt)
            returning id, user_id, created_at, updated_at
        ";

        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryFirstAsync<V1CartDal>(new CommandDefinition(
            sql, cart, cancellationToken: token));

        return res;
    }
}
