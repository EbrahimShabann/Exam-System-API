using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exam_System.Models.ModelsConfig
{
    public class ResultConfig : IEntityTypeConfiguration<Result>
    {
        public void Configure(EntityTypeBuilder<Result> builder)
        {
            builder.HasIndex(p => new { p.ApplicationUserId, p.ExamId }).IsUnique();

            builder.Property(p => p.SubmittedAt).HasDefaultValueSql("GETDATE()");
        }
    }
}
