public interface ICartRepository
{
    Task<V1CartDal> GetByUserId(long userId, CancellationToken token);

    Task<V1CartDal> GetOrCreateByUserId(long userId, CancellationToken token);

    Task<V1CartDal> Create(V1CartDal cart, CancellationToken token);
}
