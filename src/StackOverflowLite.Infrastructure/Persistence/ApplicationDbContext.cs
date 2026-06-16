using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StackOverflowLite.Application.Common.Interfaces;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Question> Questions => Set<Question>();

    public DbSet<Answer> Answers => Set<Answer>();

    public DbSet<Vote> Votes => Set<Vote>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<QuestionTag> QuestionTags => Set<QuestionTag>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.Reputation)
                .HasDefaultValue(0);
        });
    }
}