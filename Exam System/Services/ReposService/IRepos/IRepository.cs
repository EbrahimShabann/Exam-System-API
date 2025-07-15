using System.Linq.Expressions;

namespace Exam_System.Services.ReposService.IRepos
{
    public interface IRepository<T> where T:class
    {
        T GetById(int id) ;
        IEnumerable<T> GetAll() ;
        void Add(T entity) ;
        void Update(T entity);
        void Delete(T entity);
        void RemoveRange(IEnumerable<T> entities);
        IEnumerable<T> GetAll(
        Expression<Func<T, bool>>? filter = null,
        params string[] includes);

        T GetById(
            int id,
            params string[] includes);

    }
}
