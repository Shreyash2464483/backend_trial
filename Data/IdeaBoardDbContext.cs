using backend_trial.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace backend_trial.Data
{
    public class IdeaBoardDbContext : DbContext
    {
        public IdeaBoardDbContext(DbContextOptions<IdeaBoardDbContext> options) : base(options)
        {
        }


        // DbSets
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Idea> Ideas => Set<Idea>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Vote> Votes => Set<Vote>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enum converters -> store enum values as strings in SQL
            var userRoleConv = new EnumToStringConverter<UserRole>();
            var userStatusConv = new EnumToStringConverter<UserStatus>();
            var ideaStatusConv = new EnumToStringConverter<IdeaStatus>();
            var voteTypeConv = new EnumToStringConverter<VoteType>();
            var reviewDecisionConv = new EnumToStringConverter<ReviewDecision>();
            var notificationTypeConv = new EnumToStringConverter<NotificationType>();
            var notificationStatusConv = new EnumToStringConverter<NotificationStatus>();

            // Category entity configuration
            modelBuilder.Entity<Category>(e =>
            {
                e.ToTable("Category");
                e.HasKey(x => x.CategoryId);

                e.Property(x => x.CategoryId)
                    .HasDefaultValueSql("NEWSEQUENTIALID()");

                e.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(x => x.Description)
                    .HasMaxLength(500);

                e.Property(x => x.IsActive)
                    .IsRequired();

                // One category name should be unique
                e.HasIndex(x => x.Name).IsUnique();

                // Relationship: Category (1) -> Idea (many)
                // Configured on Idea entity to keep FK rules in one place.
            });

            // User entity configuration
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("User");
                e.HasKey(x => x.UserId);

                e.Property(x => x.UserId)
                    .HasDefaultValueSql("NEWSEQUENTIALID()");

                e.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                e.HasIndex(x => x.Email)
                    .IsUnique();

                e.Property(x => x.PasswordHash)
                    .IsRequired();

                e.Property(x => x.Role)
                    .HasConversion(userRoleConv)
                    .IsRequired();

                e.Property(x => x.Status)
                    .HasConversion(userStatusConv)
                    .IsRequired()
                    .HasDefaultValue(UserStatus.Active);
            });

            // Idea entity configuration
            modelBuilder.Entity<Idea>(e =>
            {
                e.ToTable("Idea");
                e.HasKey(x => x.IdeaId);

                e.Property(x => x.IdeaId)
                    .HasDefaultValueSql("NEWSEQUENTIALID()");

                e.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                e.Property(x => x.Description)
                    .IsRequired();

                e.Property(x => x.SubmittedDate)
                    .IsRequired();

                e.Property(x => x.Status)
                    .HasConversion(ideaStatusConv)
                    .IsRequired()
                    .HasMaxLength(30);

                // FK: Idea.CategoryId -> Category.CategoryId
                e.HasOne(x => x.Category)
                    .WithMany() // Category class currently has no Ideas navigation in your code; if you add ICollection<Idea> Ideas then change to .WithMany(c => c.Ideas)
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // FK: Idea.SubmittedByUserId -> User.UserId
                e.HasOne(x => x.SubmittedByUser)
                    .WithMany(u => u.SubmittedIdeas)
                    .HasForeignKey(x => x.SubmittedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.CategoryId);
                e.HasIndex(x => x.SubmittedByUserId);
                e.HasIndex(x => x.SubmittedDate);
                e.HasIndex(x => x.Status);
            });

            // Comment entity configuration
            modelBuilder.Entity<Comment>(e =>
            {
                e.ToTable("Comment");
                e.HasKey(x => x.CommentId);

                e.Property(x => x.CommentId)
                    .HasDefaultValueSql("NEWSEQUENTIALID()");

                e.Property(x => x.Text)
                    .IsRequired();

                e.Property(x => x.CreatedDate)
                    .IsRequired();

                // FK: Comment.IdeaId -> Idea.IdeaId
                e.HasOne(x => x.Idea)
                    .WithMany(i => i.Comments)
                    .HasForeignKey(x => x.IdeaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // FK: Comment.UserId -> User.UserId
                e.HasOne(x => x.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.IdeaId);
                e.HasIndex(x => x.UserId);
                e.HasIndex(x => x.CreatedDate);
            });

            // Vote entity configuration
            modelBuilder.Entity<Vote>(e =>
            {
                e.ToTable("Vote");
                e.HasKey(x => x.VoteId);

                e.Property(x => x.VoteId)
                    .HasDefaultValueSql("NEWSEQUENTIALID()");

                e.Property(x => x.VoteType)
                    .HasConversion(voteTypeConv)
                    .IsRequired()
                    .HasMaxLength(20);

                // FK: Vote.IdeaId -> Idea.IdeaId
                e.HasOne(x => x.Idea)
                    .WithMany(i => i.Votes)
                    .HasForeignKey(x => x.IdeaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // FK: Vote.UserId -> User.UserId
                e.HasOne(x => x.User)
                    .WithMany(u => u.Votes)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // One vote per user per idea (recommended)
                e.HasIndex(x => new { x.IdeaId, x.UserId }).IsUnique();
            });

            // Review entity configuration
            modelBuilder.Entity<Review>(e =>
            {
                e.ToTable("Review");
                e.HasKey(x => x.ReviewId);

                e.Property(x => x.ReviewId)
                    .HasDefaultValueSql("NEWSEQUENTIALID()");

                e.Property(x => x.Feedback)
                    .IsRequired();

                e.Property(x => x.Decision)
                    .HasConversion(reviewDecisionConv)
                    .IsRequired()
                    .HasMaxLength(20);

                e.Property(x => x.ReviewDate)
                    .IsRequired();

                // FK: Review.IdeaId -> Idea.IdeaId
                e.HasOne(x => x.Idea)
                    .WithMany(i => i.Reviews)
                    .HasForeignKey(x => x.IdeaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // FK: Review.ReviewerId -> User.UserId
                e.HasOne(x => x.Reviewer)
                    .WithMany(u => u.ReviewsAuthored)
                    .HasForeignKey(x => x.ReviewerId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.IdeaId);
                e.HasIndex(x => x.ReviewerId);
                e.HasIndex(x => x.ReviewDate);
                e.HasIndex(x => x.Decision);
            });

            // Notification entity configuration
            modelBuilder.Entity<Notification>(e =>
            {
                e.ToTable("Notification");
                e.HasKey(x => x.NotificationId);

                e.Property(x => x.NotificationId)
                    .HasDefaultValueSql("NEWSEQUENTIALID()");

                e.Property(x => x.Type)
                    .HasConversion(notificationTypeConv)
                    .IsRequired()
                    .HasMaxLength(30);

                e.Property(x => x.Status)
                    .HasConversion(notificationStatusConv)
                    .IsRequired()
                    .HasMaxLength(20);

                e.Property(x => x.Message)
                    .IsRequired()
                    .HasMaxLength(500);

                e.Property(x => x.CreatedDate)
                    .IsRequired();

                // FK: Notification.UserId -> User.UserId
                e.HasOne(x => x.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // FK: Notification.IdeaId -> Idea.IdeaId
                e.HasOne(x => x.Idea)
                    .WithMany()
                    .HasForeignKey(x => x.IdeaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // FK: Notification.ReviewerId -> User.UserId
                e.HasOne(x => x.Reviewer)
                    .WithMany()
                    .HasForeignKey(x => x.ReviewerId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.UserId, x.Status });
                e.HasIndex(x => x.CreatedDate);
            });

            // Report entity configuration
            modelBuilder.Entity<Report>(e =>
            {
                e.ToTable("Report");
                e.HasKey(x => x.ReportId);

                e.Property(x => x.ReportId)
                    .HasDefaultValueSql("NEWSEQUENTIALID()");

                e.Property(x => x.Scope)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(x => x.Metrics)
                    .IsRequired();

                e.Property(x => x.GeneratedDate)
                    .IsRequired();

                e.HasIndex(x => x.GeneratedDate);
            });

        }
    }
}
