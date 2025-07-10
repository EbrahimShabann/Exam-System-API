using Exam_System.Models;
using Exam_System.Services.DTOs;
using Exam_System.Services.ReposService.IRepos;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Exam_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IUnitOfWork uof;

        public QuestionsController(IUnitOfWork uof)
        {
            this.uof = uof;
        }
        [HttpPost("AddQues")]
        public IActionResult AddQuestion([FromBody] UpsertQuesDto quesDto)
        {
            if(quesDto == null | string.IsNullOrWhiteSpace(quesDto.QuestionText) || string.IsNullOrWhiteSpace(quesDto.QuestionType))
                return BadRequest("Question data is required.");
            var examFromDb= uof.ExamRepo.GetById(quesDto.ExamId);
           
            var NewQues = new Question
            {
                QuestionText = quesDto.QuestionText,
                QuestionType = Enum.TryParse<QuestionType>(quesDto.QuestionType, true, out var questionType) ? questionType : QuestionType.Text,
                CreatedAt = DateTime.Now,
                ExamId = quesDto.ExamId,

            };
            uof.QuesRepo.Add(NewQues);
            uof.Save();
            //return CreatedAtAction(nameof(GetQuestionById), new { id = NewQues.Id }, NewQues);
            return Ok(NewQues);


        }
  
        [HttpPost("AddChoice")]
        public IActionResult AddChoice([FromBody] AddChoiceDto choiceDto)
        {
            if(choiceDto == null | string.IsNullOrWhiteSpace(choiceDto.ChoiceText) )
                return BadRequest("choice data is required.");
            var quesFromDb= uof.QuesRepo.GetById(choiceDto.QuestionId);
            if(quesFromDb.QuestionType!= QuestionType.MCQ && quesFromDb.QuestionType != QuestionType.TF)
                return BadRequest($"Question with ID: {choiceDto.QuestionId} is Text Ques and Choices can only be added to MCQ or TF questions.");

            var NewChoice = choiceDto.Adapt<Choice>();
            
            uof.ChoiceRepo.Add(NewChoice);
            uof.Save();
            //return CreatedAtAction(nameof(GetQuestionById), new { id = NewQues.Id }, NewQues);
            return Ok(NewChoice);


        }

        [HttpPut ]
        public IActionResult UpdateQuestion([FromBody] UpsertQuesDto quesDto, int id)
        {
            if (quesDto == null)
                return BadRequest("Question data is required.");
            var quesFromDb = uof.QuesRepo.GetById(id);
            if (quesFromDb == null)
                return NotFound($"Question with ID: {id} not found.");

            // Only update fields if they are not null or empty
            if (!string.IsNullOrWhiteSpace(quesDto.QuestionText))
                quesFromDb.QuestionText = quesDto.QuestionText;

            if (!string.IsNullOrWhiteSpace(quesDto.QuestionType))
            {
                if (Enum.TryParse<QuestionType>(quesDto.QuestionType, true, out var questionType))
                    quesFromDb.QuestionType = questionType;
            }

            // Only update ExamId if it's not zero (assuming zero is not a valid ExamId)
            if (quesDto.ExamId != 0)
                quesFromDb.ExamId = quesDto.ExamId;

            uof.QuesRepo.Update(quesFromDb);
            uof.Save();
            return Ok(quesFromDb);
        }
    }
}
