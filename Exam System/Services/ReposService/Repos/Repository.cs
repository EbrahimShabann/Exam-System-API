using Exam_System.Services.ReposService.IRepos;

namespace Exam_System.Services.ReposService.Repos
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext db;

        public Repository(AppDbContext db)
        {
            this.db = db;
        }
        public void Add(T entity)
        {
           db.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
           db.Set<T>().Remove(entity);
        }

        public IEnumerable<T> GetAll()
        {
            return db.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            return db.Set<T>().Find(id) ?? throw new KeyNotFoundException($"Entity of type {typeof(T).Name} with ID {id} not found.");
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
           db.Set<T>().RemoveRange(entities);
        }

        public void Update(T entity)
        {
            db.Set<T>().Update(entity);
        }
    }
}
