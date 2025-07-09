using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam_System.Models
{
    public class StudentAnswer
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser User { get; set; }


        [Required]
        public int ExamId { get; set; }
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }


        [Required]
        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question Question{ get; set; }


        
        public int? ChoiceId { get; set; }
        [ForeignKey("ChoiceId")]
        public virtual Choice Choice{ get; set; }


        public string TextAnswer { get; set; }
        public bool? IsCorrect { get; set; }

        public DateTime AnsweredAt { get; set; }

    }
}
