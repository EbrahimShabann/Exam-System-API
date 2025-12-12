using Exam_System.Models;
using ExamSystem.Models;

namespace Exam_System.Services.ReposService.IRepos
{
    public interface IUnitOfWork
    {
        public IRepository<Exam> ExamRepo { get; }
        public IRepository<Question> QuesRepo { get;  }
        public IRepository<Choice> ChoiceRepo { get;  }
        public IRepository<StudentAnswer> StudentAnswerRepo { get; }
        public IRepository<Result> ResultRepo { get; }
        public IRepository<OTP> OTPsRepo { get; }
        Task SaveAsync();
    }
}
