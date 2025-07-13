using Exam_System.Models;
using Exam_System.Services.ReposService.IRepos;
using Microsoft.AspNetCore.Localization;

namespace Exam_System.Services.ReposService.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext db;
        IRepository<Exam> examRepo;
        IRepository<Question> quesRepo;
        IRepository<Choice> choiceRepo;
        IRepository<StudentAnswer> studentAnswerRepo;
        IRepository<Result> resultRepo;
        public UnitOfWork(AppDbContext db)
        {
            this.db = db;
        }
        public IRepository<Exam> ExamRepo
        { 
            get
            {
                if (examRepo != null) return examRepo;
                examRepo = new Repository<Exam>(db);
                return examRepo;
            }
                
        }
        public IRepository<Question> QuesRepo
        {
            get
            {
                if (quesRepo != null) return quesRepo;
                quesRepo = new Repository<Question>(db);
                return quesRepo;
            }
                
         }

        public IRepository<Choice> ChoiceRepo
        {
            get
            {
                if (choiceRepo != null) return choiceRepo;
                choiceRepo = new Repository<Choice>(db);
                return choiceRepo;
            }
        }

        public IRepository<StudentAnswer> StudentAnswerRepo
        {
            get
            {
                if (studentAnswerRepo != null) return studentAnswerRepo;
                studentAnswerRepo = new Repository<StudentAnswer>(db);
                return studentAnswerRepo;
            }
        }
        public IRepository<Result> ResultRepo
        {
            get
            {
                if (resultRepo != null) return resultRepo;
                resultRepo = new Repository<Result>(db);
                return resultRepo;
            }
        }

        public void Save()
        {
            db.SaveChanges();
        }

    }
}
