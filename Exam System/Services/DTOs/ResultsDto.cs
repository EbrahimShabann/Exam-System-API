using Exam_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam_System.Services.DTOs
{
    public class ResultsDto
    {

            public int Id { get; set; }

            public int ExamId { get; set; }
            public Exam Exam { get; set; }
            public float Degree { get; set; }

            public DateTime SubmittedAt { get; set; }
        
    }



}

