using Exam_System.Models;
using Exam_System.Services.DTOs;
using Exam_System.Services.ReposService.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Exam_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        
        private readonly IUnitOfWork uof;
        private readonly UserManager<ApplicationUser> userManager;

        public ExamController(IUnitOfWork uof, UserManager<ApplicationUser> userManager)
        {
          
            this.uof = uof;
            this.userManager = userManager;
        }
        [HttpGet("GetAllExams")]
        public IActionResult GetExams()
        {
            var exams = uof.ExamRepo.GetAll().Select(e => new ExamDto
            {
                Id=e.Id,
                Title=e.Title,
                Description=e.Description,
                Duration=e.Duration,
                CreatedBy = userManager.FindByIdAsync(e.ApplicationUserId).Result.UserName,
                
                //Questions
            }).ToList();
            return exams != null ? Ok(exams) : NotFound("No exams found.");

        }

        [HttpGet("{id}")]
        public IActionResult GetExamById(int id)
        {
            var exam = uof.ExamRepo.GetById(id);
            return exam != null ? Ok(exam) : NotFound($"Exam With Id: {id} Not Found ");
        }

        [HttpPost]
        //[Authorize(Roles = "Teacher")]
        public IActionResult CreateExam(UpsertExamDTO examModel)
        {
            if (examModel == null)
            {
                return BadRequest("Exam cannot be null.");
            }
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type} => {claim.Value}");
            }
            //create exam has  questions and choices for each question
            var exam = new Exam()
            {
                Title = examModel.Title,
                Description = examModel.Description,
                ApplicationUserId = "509f5459-2c9e-4623-a63b-42c69b9fb698", //User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value,
                CreatedAt = DateTime.Now,
                Duration = examModel.Duration,
                Questions = examModel.Questions.Select(q => new Question
                {
                    QuestionText = q.QuestionText,
                    QuestionType = Enum.TryParse<QuestionType>(q.QuestionType, true, out var questionType) ? questionType : QuestionType.Text,
                    CreatedAt = DateTime.Now,
                    Choices = q.Choices.Select(c => new Choice
                    {
                        ChoiceText = c.ChoiceText,
                        IsCorrect = c.IsCorrect
                    }).ToList()
                }).ToList()

            };
            
            uof.ExamRepo.Add(exam);  
            uof.Save();
            return CreatedAtAction(nameof(GetExamById), new { id = exam.Id }, exam);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public IActionResult EditExam([FromBody]UpsertExamDTO updatedExam,[FromRoute] int id)
        {
            var examFromDb = uof.ExamRepo.GetById(id);
            if (examFromDb == null) return BadRequest();
            examFromDb.Title = updatedExam.Title;
            examFromDb.Description= updatedExam.Description;
            examFromDb.Duration= updatedExam.Duration;
            uof.ExamRepo.Update(examFromDb);
            uof.Save();
            return Ok(examFromDb);

        }
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Teacher")]
        public IActionResult DeleteExam([FromRoute]int id)
        {
            var examFromDb = uof.ExamRepo.GetById(id);
            if (examFromDb == null) return NotFound(new { message = $"Exam with Id: {id} Not Found." });
            var quesOfExam=uof.QuesRepo.GetAll().Where(q=>q.ExamId==id);
            var choicesofQues= uof.ChoiceRepo.GetAll().Where(c=>quesOfExam.Select(q=>q.Id).ToList().Contains(c.QuestionId)); //get all choices related to the questions of the exam
            uof.ChoiceRepo.RemoveRange(choicesofQues);
            uof.QuesRepo.RemoveRange(quesOfExam);     //delete all questions related to the exam
            uof.ExamRepo.Delete(examFromDb);
            uof.Save();
            return Ok(new { message = $"Exam with Id: {id} deleted successfully." });

        }
    }
}
