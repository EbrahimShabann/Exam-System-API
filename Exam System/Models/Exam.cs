using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam_System.Models
{
    
    public class Exam
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100,ErrorMessage ="Exam title can't exceed 100 chars")]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(10,240,ErrorMessage ="Exam Duration Range between 10 and 240 Mins")]
        public int Duration { get; set; }

        [Column("CreatedBy")]
        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser Teacher { get; set; } 

        public virtual List<Question> Questions { get; set; } = [];
        public virtual List<Result> Results { get; set; } = [];
        public DateTime CreatedAt { get; set; }
    }
}
