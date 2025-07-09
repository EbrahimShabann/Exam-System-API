using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam_System.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public int ExamId { get; set; }
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }

        [Required]
        public string QuestionText { get; set; }


        [Required]
        public QuestionType QuestionType { get; set; }


        public DateTime CreatedAt { get; set; }

        public virtual List<Choice> Choices { get; set; }
    }

    public enum QuestionType
    {
        MCQ,
        TF,
        Text
    }
}
