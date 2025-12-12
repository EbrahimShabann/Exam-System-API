using Exam_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamSystem.Models
{
    public class OTP
    {
        public int Id { get; set; }
        public string OTPCode { get; set; }                                     
        public bool IsValid { get; set; }
        public DateTime OTPGeneratedAt { get; set; }
        public string ApplicationUserId { get; set; }
        public string UserEmail { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser User { get; set; }

    }
}
