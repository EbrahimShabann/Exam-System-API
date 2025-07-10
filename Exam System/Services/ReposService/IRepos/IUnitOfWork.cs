using Exam_System.Models;

namespace Exam_System.Services.ReposService.IRepos
{
    public interface IUnitOfWork
    {
        public IRepository<Exam> ExamRepo { get; }
        public IRepository<Question> QuesRepo { get;  }
        public IRepository<Choice> ChoiceRepo { get;  }
        void Save();
    }
}
