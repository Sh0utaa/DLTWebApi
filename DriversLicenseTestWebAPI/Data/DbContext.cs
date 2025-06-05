using DriversLicenseTestWebAPI.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthDemo.Data;

public class DataContext : IdentityDbContext<IdentityUser>
{
    public DataContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Question>()
        .HasMany(q => q.Answers)
        .WithOne(a => a.Question)
        .HasForeignKey(a => a.QuestionId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserAnswerSubmission>().HasKey(x => x.Id);

        builder.Entity<UserAnswerSubmission>().Property(x => x.SubmittedAt).HasDefaultValueSql("GETDATE()");

        builder.Entity<ExamSession>()
            .HasMany(s => s.Answers)
            .WithOne(a => a.ExamSession)
            .HasForeignKey(a => a.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
