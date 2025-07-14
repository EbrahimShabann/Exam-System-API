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

        public virtual List<UpsertQuesDtoWithExam> Questions { get; set; } = [];
        public virtual List<Result> Results { get; set; } = [];
       
    }

    public class SubmitExamDto
    {
        public List<SubmitAnswerDto> Answers { get; set; }
    }
    public class SubmitAnswerDto
    {
        public int QuestionId { get; set; }
        public int? ChoiceId { get; set; }
        public string? TextAnswer { get; set; }
        public bool? BoolAnswer { get; set; } // for TF questions
    }
}

