using System.Text.Json.Serialization;

namespace Exam_System.Services.DTOs
{
    public class UpsertQuesDtoWithExam
    {
        [JsonPropertyName("questionText")]
        public string QuestionText { get; set; }

        [JsonPropertyName("questionType")]
        public string QuestionType { get; set; }
        public bool? TFCorrectAnswer { get; set; } // True = True is correct, False = False is correct
        public List<UpsertChoiceDtoWithQues> Choices { get; set; }

    }
}
