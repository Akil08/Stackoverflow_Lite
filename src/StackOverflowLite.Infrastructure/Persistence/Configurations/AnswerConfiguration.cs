using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Infrastructure.Persistence.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.Property(answer => answer.Content)
            .IsRequired();

        builder.Property(answer => answer.IsAccepted)
            .HasDefaultValue(false);

        builder.HasOne(answer => answer.Author)
            .WithMany(author => author.Answers)
            .HasForeignKey(answer => answer.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}