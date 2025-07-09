using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exam_System.Models.ModelsConfig
{
    public class StudentAnswerConfig : IEntityTypeConfiguration<StudentAnswer>
    {
        public void Configure(EntityTypeBuilder<StudentAnswer> builder)
        {
           
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_StudentAnswers_ChoiceOrTextAnswer",
                "(ChoiceId IS NOT NULL AND TextAnswer IS NULL) OR (ChoiceId IS NULL AND TextAnswer IS NOT NULL)");
            });
        
        }
    }
}
