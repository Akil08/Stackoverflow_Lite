using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Infrastructure.Persistence.Configurations;

public class QuestionTagConfiguration : IEntityTypeConfiguration<QuestionTag>
{
    public void Configure(EntityTypeBuilder<QuestionTag> builder)
    {
        builder.HasKey(questionTag => new { questionTag.QuestionId, questionTag.TagId });

        builder.HasOne(questionTag => questionTag.Question)
            .WithMany(question => question.QuestionTags)
            .HasForeignKey(questionTag => questionTag.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(questionTag => questionTag.Tag)
            .WithMany(tag => tag.QuestionTags)
            .HasForeignKey(questionTag => questionTag.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}