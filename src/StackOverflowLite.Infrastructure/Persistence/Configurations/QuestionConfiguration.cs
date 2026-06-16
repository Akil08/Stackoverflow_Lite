using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.Property(question => question.Title)
            .IsRequired();

        builder.Property(question => question.Description)
            .IsRequired();

        builder.Property(question => question.ViewCount)
            .HasDefaultValue(0);

        builder.HasOne(question => question.Author)
            .WithMany(author => author.Questions)
            .HasForeignKey(question => question.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(question => question.Answers)
            .WithOne(answer => answer.Question)
            .HasForeignKey(answer => answer.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(question => question.QuestionTags)
            .WithOne(questionTag => questionTag.Question)
            .HasForeignKey(questionTag => questionTag.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Answer>()
            .WithMany()
            .HasForeignKey(question => question.AcceptedAnswerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}