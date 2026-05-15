using Dapper;
using WebApi.DAL;

public class UserRepository(UnitOfWork unitOfWork) : IUserRepository
{
    public async Task<V1UserDal> GetByUsername(string username, CancellationToken token)
    {
        var sql = @"
            select 
                id,
                username,
                email,
                password_hash,
                created_at,
                updated_at
            from users
            where username = @Username
        ";

        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryFirstOrDefaultAsync<V1UserDal>(new CommandDefinition(
            sql, new { Username = username }, cancellationToken: token));

        return res;
    }

    public async Task<V1UserDal> GetByEmail(string email, CancellationToken token)
    {
        var sql = @"
            select 
                id,
                username,
                email,
                password_hash,
                created_at,
                updated_at
            from users
            where email = @Email
        ";

        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryFirstOrDefaultAsync<V1UserDal>(new CommandDefinition(
            sql, new { Email = email }, cancellationToken: token));

        return res;
    }

    public async Task<V1UserDal> GetById(long id, CancellationToken token)
    {
        var sql = @"
            select 
                id,
                username,
                email,
                password_hash,
                created_at,
                updated_at
            from users
            where id = @Id
        ";

        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryFirstOrDefaultAsync<V1UserDal>(new CommandDefinition(
            sql, new { Id = id }, cancellationToken: token));

        return res;
    }

    public async Task<V1UserDal> Create(V1UserDal user, CancellationToken token)
    {
        var sql = @"
            insert into users (username, email, password_hash, created_at, updated_at)
            values (@Username, @Email, @PasswordHash, @CreatedAt, @UpdatedAt)
            returning id, username, email, password_hash, created_at, updated_at
        ";

        var conn = await unitOfWork.GetConnection(token);
        var res = await conn.QueryFirstAsync<V1UserDal>(new CommandDefinition(
            sql, user, cancellationToken: token));

        return res;
    }
}
