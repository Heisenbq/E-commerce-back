public interface IProductRepository
{
    Task<V1ProductDal[]> Query(QueryProductsDalModel model, CancellationToken token);

    Task<long> QueryTotal(QueryProductsDalModel model, CancellationToken token);

    Task<V1ProductDal> GetById(long id, CancellationToken token);

    Task<V1ProductDal> BulkInsert(V1ProductDal product, CancellationToken token);
}
