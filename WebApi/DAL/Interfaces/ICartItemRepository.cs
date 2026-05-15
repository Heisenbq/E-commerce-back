public interface ICartItemRepository
{
    Task<V1CartItemDal[]> GetByCartId(long cartId, CancellationToken token);

    Task<V1CartItemDal> GetById(long id, CancellationToken token);

    Task<V1CartItemDal> AddItem(V1CartItemDal item, CancellationToken token);

    Task UpdateQuantity(long cartItemId, int quantity, CancellationToken token);

    Task DeleteItems(long[] cartItemIds, CancellationToken token);

    Task DeleteByCartId(long cartId, CancellationToken token);
}
