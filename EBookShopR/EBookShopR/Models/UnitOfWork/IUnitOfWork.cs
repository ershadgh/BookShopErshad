using BookShop.Models.Repository;

namespace BookShop.Models.UnitOfWork
{
    public interface IUnitOfWork
    {
        BookShopContext _Context { get; }
        IBooksRepository booksRepository { get; }
        IRepositoryBase<TEntity> BaseRepository<TEntity>() where TEntity : class;
        Task Cmmit();
    }
}
