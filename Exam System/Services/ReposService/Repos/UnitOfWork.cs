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

        public void Save()
        {
            db.SaveChanges();
        }

    }
}
