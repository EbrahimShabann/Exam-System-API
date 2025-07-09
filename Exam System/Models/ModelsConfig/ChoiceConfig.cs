using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exam_System.Models.ModelsConfig
{
    public class ChoiceConfig : IEntityTypeConfiguration<Choice>
    {
        public void Configure(EntityTypeBuilder<Choice> builder)
        {
            builder.Property(p => p.IsCorrect).HasDefaultValueSql("False");
        }
    }
}
