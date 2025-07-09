using Exam_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Exam_System.Services
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Choice> Choices{ get; set; }
        public DbSet<StudentAnswer> StudentAnswers{ get; set; }
        public DbSet<Result> Results { get; set; }
        public AppDbContext(DbContextOptions options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(Exam).Assembly);
            base.OnModelCreating(builder);
        }
      
    }
}
