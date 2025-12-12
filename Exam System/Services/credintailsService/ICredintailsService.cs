using Exam_System.Services.DTOs;
using ExamSystem.Models;

namespace ExamSystem.Services.credintailsService
{
    public interface ICredintailsService
    {
        Task<(bool success, string message, string otp)> registerAsync(RegisterDto dto);
        Task<OTP> GenerateOTP(string userEmail);
        Task<bool> ValidateOTPAsync(string email, string otp);
       
    }
}
