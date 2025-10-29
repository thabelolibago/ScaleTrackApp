using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Database
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }

        public DbSet<Issue> Issues { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<IssueTag> IssueTags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================
            // Identity table renaming
            // ========================
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

            // ========================
            // Enum conversions
            // ========================
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Issue>()
                .Property(i => i.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Issue>()
                .Property(i => i.Priority)
                .HasConversion<string>();

            modelBuilder.Entity<Issue>()
                .Property(i => i.Status)
                .HasConversion<string>();

            // ========================
            // Relationships
            // ========================
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Issue)
                .WithMany(i => i.Comments)
                .HasForeignKey(c => c.IssueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IssueTag>()
                .HasKey(it => new { it.IssueId, it.TagId });

            modelBuilder.Entity<IssueTag>()
                .HasOne(it => it.Issue)
                .WithMany(i => i.IssueTags)
                .HasForeignKey(it => it.IssueId);

            modelBuilder.Entity<IssueTag>()
                .HasOne(it => it.Tag)
                .WithMany(t => t.IssueTags)
                .HasForeignKey(it => it.TagId);

            modelBuilder.Entity<AuditTrail>()
                .HasOne(a => a.ChangedByUser)
                .WithMany(u => u.ChangesMade)
                .HasForeignKey(a => a.ChangedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<AuditTrail>()
                .HasOne(a => a.ApprovedByUser)
                .WithMany(u => u.ApprovalsGiven)
                .HasForeignKey(a => a.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}

