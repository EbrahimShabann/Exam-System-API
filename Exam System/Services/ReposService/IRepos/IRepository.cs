namespace Exam_System.Services.ReposService.IRepos
{
    public interface IRepository<T> where T:class
    {
        T GetById(int id) ;
        IEnumerable<T> GetAll() ;
        void Add(T entity) ;
        void Update(T entity);
        void Delete(T entity);
        
    }
}
