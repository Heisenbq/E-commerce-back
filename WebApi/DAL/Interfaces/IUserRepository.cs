public interface IUserRepository
{
    Task<V1UserDal> GetByUsername(string username, CancellationToken token);

    Task<V1UserDal> GetByEmail(string email, CancellationToken token);

    Task<V1UserDal> GetById(long id, CancellationToken token);

    Task<V1UserDal> Create(V1UserDal user, CancellationToken token);
}
