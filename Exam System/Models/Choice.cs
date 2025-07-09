using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam_System.Models
{
    public class Choice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }


        [Required]
        public string ChoiceText { get; set; }

        [Required]
        public bool IsCorrect { get; set; }
    }
}
