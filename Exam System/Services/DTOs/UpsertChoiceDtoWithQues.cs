using System.Text.Json.Serialization;

namespace Exam_System.Services.DTOs
{
    public class UpsertChoiceDtoWithQues
    {

        [JsonPropertyName("choiceText")]
        public string ChoiceText { get; set; }

        [JsonPropertyName("isCorrect")]
        public bool IsCorrect { get; set; }
    }
}
