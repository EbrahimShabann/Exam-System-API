using Exam_System.Services.ReposService.IRepos;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Services
{
    public class BGJobsService
    {
        private readonly IUnitOfWork unitOfWork;

        public BGJobsService(IUnitOfWork unitOfWork )
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task ClearOtbs()
        {
            //get old and not valid otps of users  
            var otps = unitOfWork.OTPsRepo.GetAll(o => (!o.IsValid  || EF.Functions.DateDiffMinute(o.OTPGeneratedAt, DateTime.UtcNow) >= 5)).ToList();
            unitOfWork.OTPsRepo.RemoveRange(otps);
            await unitOfWork.SaveAsync();
            Console.WriteLine("Clear InValid OTPs using BackGround Job");
        }

    }
}
