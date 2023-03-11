using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookShop.Models.Repository
{
    public class RepositoryBase<TEntity,TContext>: IRepositoryBase<TEntity> where TEntity : class where TContext:DbContext
    {
       
        public RepositoryBase()
        {

        }
        protected TContext _context { get; set; }
        private DbSet<TEntity> dbset;
        public RepositoryBase(TContext context)
        {
            _context = context;
            dbset = context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> FindAllAsync()
        {
            return await dbset.AsNoTracking().ToListAsync();   
        }
        public IEnumerable<TEntity> FindAll()
        {
            return dbset.AsNoTracking().ToList();
            
        }
        public async Task<TEntity> FindByIDAsync(object id)
        {
            return await dbset.FindAsync(id);
        }
        public async Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity,bool>> filter=null,Func<IQueryable<TEntity>,IOrderedQueryable<TEntity>> orderBy=null,params Expression<Func<TEntity,object>>[] includes)
        {
            IQueryable<TEntity> query = dbset;
            foreach (var include in includes)
            {
                query=query.Include(include);
            }
            if (filter!=null)
            {
                query = query.Where(filter);
            }
            if (orderBy!=null)
            {
                query = orderBy(query);
            }
            return await query.ToListAsync();
        }
        public async Task Create(TEntity entity)
        {
            await dbset.AddAsync(entity);
        }
        public void Update(TEntity entity) => dbset.Update(entity);


        public void Delete(TEntity entity)=> dbset.Remove(entity);
        public async Task CreateRange(IEnumerable<TEntity> entities) => await dbset.AddRangeAsync(entities);
        public async void UpdateRange(IEnumerable<TEntity> entities) =>  dbset.UpdateRange(entities);
        public async void DeleteRange(IEnumerable<TEntity> entities)=> dbset.RemoveRange(entities);
        public async Task<List<TEntity>> GetpaginteResultAsync(int CurrentPage,int PageSize=1)
        {
            var Entities = await FindAllAsync();
            return Entities.Skip((CurrentPage-1)*PageSize).Take(PageSize).ToList();
        }
        public int Getcount()
        {
            return dbset.Count();
        }

       
    }
}
