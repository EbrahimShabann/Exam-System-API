using Exam_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam_System.Services.DTOs
{
    public class UpsertExamDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }

        
        public int Duration { get; set; }


       
    }
}
