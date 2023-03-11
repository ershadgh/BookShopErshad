using BookShop.Models.Repository;

namespace BookShop.Models.UnitOfWork
{
    public class UnitOfWork:IUnitOfWork
    {
        public BookShopContext _Context { get; set; }
        private IBooksRepository _IBooksRepository;
        public UnitOfWork(BookShopContext context)
        {
            _Context = context; 
        }
        
       public IRepositoryBase<TEntity> BaseRepository<TEntity>() where TEntity:class
        {
            IRepositoryBase<TEntity> repository=new RepositoryBase<TEntity,BookShopContext>(_Context);
            return repository;
        }
        public IBooksRepository booksRepository
        {
            get
            {
                if (_IBooksRepository == null)
                {
                    _IBooksRepository = new BooksRepository(_Context);
                }
                return _IBooksRepository;
            }
        }
        public async Task Cmmit()
        {
            await _Context.SaveChangesAsync();
        }
        
      
    }
}
