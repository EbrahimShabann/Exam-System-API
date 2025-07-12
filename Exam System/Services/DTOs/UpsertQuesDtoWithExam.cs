using System.Text.Json.Serialization;

namespace Exam_System.Services.DTOs
{
    public class UpsertQuesDtoWithExam
    {

        public string QuestionText { get; set; }
        public string QuestionType { get; set; }

        public List<UpsertChoiceDtoWithQues> Choices { get; set; }

    }
}
