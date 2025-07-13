using Exam_System.Models;
using Exam_System.Services.DTOs;
using Exam_System.Services.ReposService.IRepos;
using Mapster;
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

            var config = new TypeAdapterConfig();
            config.NewConfig<Question, UpsertQuesDtoWithExam>()
                  .Map(dest => dest.QuestionType, src => src.QuestionType.ToString());

            var examDto = exam.Adapt<ExamDto>(config);

            return examDto != null ? Ok(examDto) : NotFound($"Exam With Id: {id} Not Found ");
        }

        //[Authorize(Roles = "Teacher")]
        [HttpPost]
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
                ApplicationUserId = "509f5459-2c9e-4623-a63b-42c69b9fb698", //User.FindFirstValue(ClaimTypes.NameIdentifier),
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
            //update exam fields
            examFromDb.Title = updatedExam.Title;
            examFromDb.Description= updatedExam.Description;
            examFromDb.Duration= updatedExam.Duration;
            //remove old ques and choices
            var choicesofQues = uof.ChoiceRepo.GetAll().Where(c => examFromDb.Questions.Select(q => q.Id).ToList().Contains(c.QuestionId));
            uof.ChoiceRepo.RemoveRange(choicesofQues);
            uof.QuesRepo.RemoveRange(examFromDb.Questions);
            //update quess
            examFromDb.Questions = updatedExam.Questions.Select(q => new Question
            {
                QuestionText = q.QuestionText,
                QuestionType = Enum.TryParse<QuestionType>(q.QuestionType, true, out var questionType) ? questionType : QuestionType.Text,
                Choices = q.Choices.Select(c => new Choice
                {
                    ChoiceText = c.ChoiceText,
                    IsCorrect = c.IsCorrect
                }).ToList()
            }).ToList();

            uof.ExamRepo.Update(examFromDb);
            uof.Save();
            return Ok(examFromDb);

        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public IActionResult DeleteExam([FromRoute]int id)
        {
            var examFromDb = uof.ExamRepo.GetById(id);
            if (examFromDb == null) return NotFound(new { message = $"Exam with Id: {id} Not Found." });
            // Remove all student answers for this exam
            var studentAnswers = uof.StudentAnswerRepo.GetAll().Where(sa => sa.ExamId == id).ToList();
            uof.StudentAnswerRepo.RemoveRange(studentAnswers);
            // Remove all results for this exam
            var results = uof.ResultRepo.GetAll().Where(r => r.ExamId == id).ToList();
            uof.ResultRepo.RemoveRange(results);
            // Remove all choices and questions for this exam
            var quesOfExam = uof.QuesRepo.GetAll().Where(q => q.ExamId == id);
            var choicesofQues = uof.ChoiceRepo.GetAll().Where(c => quesOfExam.Select(q => q.Id).ToList().Contains(c.QuestionId));
            uof.ChoiceRepo.RemoveRange(choicesofQues);
            uof.QuesRepo.RemoveRange(quesOfExam);
            uof.ExamRepo.Delete(examFromDb);
            uof.Save();
            return Ok(new { message = $"Exam with Id: {id} deleted successfully." });

        }

        [Authorize(Roles = "Student")]
        [HttpGet("available")]
        public IActionResult GetAvailableExams(string? search = null, int page = 1, int pageSize = 10)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var examsQuery = uof.ExamRepo.GetAll()
                .Where(e => !e.Results.Any(r => r.ApplicationUserId == studentId));
            if (!string.IsNullOrWhiteSpace(search))
                examsQuery = examsQuery.Where(e => e.Title.Contains(search));
            var total = examsQuery.Count();
            var exams = examsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new { e.Id, e.Title, e.Description })
                .ToList();
            return Ok(new { total, exams });
        }

        [Authorize(Roles = "Student")]
        [HttpGet("{id}/questions")]
        public IActionResult GetExamQuestions(int id)
        {
            var exam = uof.ExamRepo.GetAll()
                .Where(e => e.Id == id)
                .Select(e => new {
                    e.Id,
                    Questions = e.Questions.Select(q => new {
                        q.Id,
                        q.QuestionText,
                        q.QuestionType,
                        Choices = q.QuestionType == QuestionType.MCQ
                            ? q.Choices.Select(c => new { c.Id, c.ChoiceText })
                            : null
                    })
                })
                .FirstOrDefault();
            if (exam == null) return NotFound();
            return Ok(exam.Questions);
        }

        public class SubmitExamDto
        {
            public List<SubmitAnswerDto> Answers { get; set; }
        }
        public class SubmitAnswerDto
        {
            public int QuestionId { get; set; }
            public int? ChoiceId { get; set; }
            public string? TextAnswer { get; set; }
        }

        [Authorize(Roles = "Student")]
        [HttpPost("{id}/submit")]
        public IActionResult SubmitExam(int id, [FromBody] SubmitExamDto dto)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exam = uof.ExamRepo.GetAll().FirstOrDefault(e => e.Id == id);
            if (exam == null) return NotFound();
            var questions = exam.Questions.ToList();
            int correctCount = 0, totalMcq = 0;
            var studentAnswers = new List<StudentAnswer>();
            foreach (var ans in dto.Answers)
            {
                var question = questions.FirstOrDefault(q => q.Id == ans.QuestionId);
                if (question == null) continue;
                bool? isCorrect = null;
                if (question.QuestionType == QuestionType.MCQ && ans.ChoiceId.HasValue)
                {
                    totalMcq++;
                    var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect);
                    isCorrect = (ans.ChoiceId == correctChoice?.Id);
                    if (isCorrect == true) correctCount++;
                }
                var studentAnswer = new StudentAnswer
                {
                    ApplicationUserId = studentId,
                    ExamId = id,
                    QuestionId = ans.QuestionId,
                    ChoiceId = ans.ChoiceId,
                    TextAnswer = ans.TextAnswer,
                    IsCorrect = isCorrect,
                    AnsweredAt = DateTime.UtcNow
                };
                studentAnswers.Add(studentAnswer);
                uof.StudentAnswerRepo.Add(studentAnswer);
            }
            double score = totalMcq > 0 ? (double)correctCount / totalMcq * 100 : 0;
            uof.ResultRepo.Add(new Result
            {
                ApplicationUserId = studentId,
                ExamId = id,
                Degree = (float)score,
                SubmittedAt = DateTime.UtcNow
            });
            uof.Save();

            // Build review structure
            var review = questions.Select(q => new {
                id = q.Id,
                text = q.QuestionText,
                type = q.QuestionType.ToString(),
                choices = q.QuestionType == QuestionType.MCQ ? q.Choices.Select(c => new { id = c.Id, text = c.ChoiceText }) : null,
                correctChoiceId = q.QuestionType == QuestionType.MCQ ? q.Choices.FirstOrDefault(c => c.IsCorrect)?.Id : null,
                selectedChoiceId = q.QuestionType == QuestionType.MCQ ? studentAnswers.FirstOrDefault(a => a.QuestionId == q.Id)?.ChoiceId : null,
                userTextAnswer = q.QuestionType == QuestionType.Text ? studentAnswers.FirstOrDefault(a => a.QuestionId == q.Id)?.TextAnswer : null
            }).ToList();

            return Ok(new { score, questions = review });
        }

        [Authorize(Roles = "Student")]
        [HttpGet("results")]
        public IActionResult GetStudentResults()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var results = uof.ResultRepo.GetAll()
                .Where(r => r.ApplicationUserId == studentId)
                .Select(r => new {
                    r.Id,
                    r.ExamId,
                    ExamTitle = r.Exam.Title,
                    r.Degree,
                    r.SubmittedAt
                })
                .ToList();
            return Ok(results);
        }
    }
}
