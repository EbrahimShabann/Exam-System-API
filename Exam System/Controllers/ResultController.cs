using Exam_System.Models;
using Exam_System.Services.DTOs;
using Exam_System.Services.ReposService.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Exam_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all endpoints

    public class ResultController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResultController(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: api/results/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyResults()
        {
            // Get user ID from JWT claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var results = _unitOfWork.ResultRepo.GetAll(
                r => r.ApplicationUserId == userId, // Use ID from token
                "Exam"
            ).Select(r => new
            {
                r.Id,
                r.ExamId,
                ExamTitle = r.Exam.Title,
                r.Degree,
                r.SubmittedAt
            });

            return Ok(results);
        }

        // GET: api/results/student/{id}
        [HttpGet("student/{id}")]
        [Authorize(Roles = "Teacher")]

        public IActionResult GetResultsByStudent(string id)
        {
            var results = _unitOfWork.ResultRepo.GetAll(
                r => r.ApplicationUserId == id,
                "Exam", "User"
            ).Select(r => new
            {
                r.Id,
                r.ExamId,
                ExamTitle = r.Exam.Title,
                StudentName = r.User.UserName,
                r.Degree,
                r.SubmittedAt
            });

            return Ok(results);
        }
        [HttpGet("All")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetAllResults()
        {
            var results = _unitOfWork.ResultRepo.GetAll().Select(r => new
            {
                r.Id,
                r.ExamId,
                ExamTitle = r.Exam.Title,
                StudentName = r.User.UserName,
                r.Degree,
                r.SubmittedAt
            });

            return Ok(results);
        }

        // GET: api/results/exam/{id}
        [HttpGet("exam/{id}")]
        [Authorize(Roles = "Teacher")]

        public IActionResult GetResultsByExam(int id)
        {
            var results = _unitOfWork.ResultRepo.GetAll(
                r => r.ExamId == id,
                "Exam", "User"
            ).Select(r => new
            {
                r.Id,
                r.ExamId,
                ExamTitle = r.Exam.Title,
                StudentName = r.User.UserName,
                r.Degree,
                r.SubmittedAt
            });

            return Ok(results);
        }
    }
}