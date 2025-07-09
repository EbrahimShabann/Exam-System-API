using Exam_System.Models;

namespace Exam_System.Services.DTOs
{
    public class ExamDto
    {
        public int Id { get; set; }

       
        public string Title { get; set; }

        public string Description { get; set; }

        
        public int Duration { get; set; }

        
        public string CreatedBy { get; set; }

        public virtual List<Question> Questions { get; set; } = [];
        public virtual List<Result> Results { get; set; } = [];
       
    }
}

