using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Infrastructure.Persistence.Configurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasIndex(vote => new { vote.UserId, vote.TargetId, vote.TargetType })
            .IsUnique();

        builder.HasOne(vote => vote.User)
            .WithMany(user => user.Votes)
            .HasForeignKey(vote => vote.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}