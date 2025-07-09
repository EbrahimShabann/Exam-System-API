using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam_System.Models
{
    public class Result
    {
        [Key]
        public int Id{ get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser User{ get; set; }


        [Required]
        public int ExamId { get; set; }
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }

        [Required]
        [Range(0,100,ErrorMessage ="Degree must be between 0 and 100")]
        public float Degree { get; set; }


        public DateTime SubmittedAt { get; set; }
    }
}
