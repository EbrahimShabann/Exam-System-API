using Exam_System.Models;
using Exam_System.Services.DTOs;
using Exam_System.Services.ReposService.IRepos;
using ExamSystem.Models;
using Hangfire;
using Microsoft.AspNetCore.Identity;

namespace ExamSystem.Services.credintailsService
{
    public class CredintailsService : ICredintailsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly BGJobsService bGJobsService;

        public CredintailsService(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager, BGJobsService bGJobsService)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.bGJobsService = bGJobsService;
        }
        public async Task<(bool success, string message, string otp)> registerAsync(RegisterDto model)
        {
            var existedUser = await userManager.FindByEmailAsync(model.Email);

            //if user is existed and confirmed
            if (existedUser is not null && existedUser.EmailConfirmed == true)
                return (false, "Email already exists", null);

            //students only register 
            var Otp = await GenerateOTP(model.Email);

            if(existedUser is null)
            {
                //if user is new one 
                var newUser = new ApplicationUser { UserName = model.Email, Email = model.Email, CreatedAt = DateTime.UtcNow };

                var result = await userManager.CreateAsync(newUser, model.Password);
                if (!result.Succeeded)
                    return (false, result.Errors.ToString(), null);
                else
                    await userManager.AddToRoleAsync(newUser, StaticDetails.StudentRole);
            }

            return (true, "OTP generated successfully", Otp.OTPCode);
        }


        // Generate OTP
        public async Task<OTP> GenerateOTP(string userEmail)
        {
            Random random = new Random();
            string otpCode = random.Next(100000, 999999).ToString(); // Generate a 6-digit OTP
            var otp = new OTP()
            {
                OTPCode = otpCode,
                OTPGeneratedAt = DateTime.UtcNow,
                IsValid = true,
                UserEmail = userEmail
            };

            unitOfWork.OTPsRepo.Add(otp);
            await unitOfWork.SaveAsync();
            return otp;
        }

        public async Task<bool> ValidateOTPAsync(string email, string otp)
        {


            // Check if the OTP matches and is within the valid time frame (e.g., 5 minutes)

            var otpFromDb = unitOfWork.OTPsRepo.GetAll().SingleOrDefault(o => o.UserEmail == email && o.OTPCode ==otp );

            if (otpFromDb is null)
                return false;  //otp is not valid

            if (!otpFromDb.IsValid || (DateTime.UtcNow - otpFromDb.OTPGeneratedAt).TotalMinutes >= 2)
                return false; //otp is not valid

            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return false; //user not found

            user.EmailConfirmed = true; // confirm email after validation


            // Clear OTP after successful validation
            otpFromDb.IsValid = false;
            otpFromDb.OTPGeneratedAt = DateTime.MinValue;
            otpFromDb.ApplicationUserId = user.Id;
            unitOfWork.OTPsRepo.Update(otpFromDb);
            await unitOfWork.SaveAsync();

            //clear old and not valid otps of users by background job every day at 12 pm
            RecurringJob.AddOrUpdate("clearOtps",() => bGJobsService.ClearOtbs(), Cron.Daily);

            
            return true;
        }

       
    }
}
