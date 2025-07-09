using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Exam_System.Models
{
    
    public class ApplicationUser :IdentityUser
    {
        public DateTime CreatedAt { get; set; }
        public virtual List<Exam> Exams { get; set; }
        public virtual List<Result> Results { get; set; }

    }
}
