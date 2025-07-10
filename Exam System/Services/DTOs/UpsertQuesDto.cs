using System.Text.Json.Serialization;

namespace Exam_System.Services.DTOs
{
    public class UpsertQuesDto
    {
       

        public int ExamId { get; set; }

        public string QuestionText { get; set; }


        public string QuestionType { get; set; }

    }
}
