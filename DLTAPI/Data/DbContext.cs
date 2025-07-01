using DLTAPI.models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthDemo.Data;

public class DataContext : IdentityDbContext<ApplicationUser>
{
    public DataContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<UserAnswerSubmission> UserAnswerSubmissions { get; set; }
    public DbSet<ExamSession> ExamSessions { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<VerifiedEmail> VerifiedEmails { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Question>()
        .HasMany(q => q.Answers)
        .WithOne(a => a.Question)
        .HasForeignKey(a => a.QuestionId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserAnswerSubmission>()
            .HasOne(u => u.Question)
            .WithMany()
            .HasForeignKey(u => u.QuestionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<UserAnswerSubmission>()
            .HasOne(u => u.Answer)
            .WithMany()
            .HasForeignKey(u => u.AnswerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<UserAnswerSubmission>()
            .HasOne(u => u.ExamSession)
            .WithMany(e => e.Answers)
            .HasForeignKey(u => u.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserAnswerSubmission>().Property(x => x.SubmittedAt).HasDefaultValueSql("GETDATE()");

        builder.Entity<ExamSession>()
            .HasMany(s => s.Answers)
            .WithOne(a => a.ExamSession)
            .HasForeignKey(a => a.SessionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<VerificationCode>()
            .Property(v => v.Type)
            .HasConversion<string>();

    }
}
