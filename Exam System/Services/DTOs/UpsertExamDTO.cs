using Exam_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Exam_System.Services.DTOs
{
    public class UpsertExamDTO
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }


        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("questions")]
        public  List<UpsertQuesDtoWithExam> Questions { get; set; } 

    }
}
