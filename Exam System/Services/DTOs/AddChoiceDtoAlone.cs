using Exam_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam_System.Services.DTOs
{
    public class AddChoiceDtoAlone
    {
        public int QuestionId { get; set; }
       
        public string ChoiceText { get; set; }

        
        public bool IsCorrect { get; set; }
    }
}
