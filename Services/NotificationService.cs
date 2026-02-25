using backend_trial.Data;
using backend_trial.Models.Domain;
using backend_trial.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend_trial.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IdeaBoardDbContext _dbContext;

        public NotificationService(IdeaBoardDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateNewIdeaNotificationAsync(Guid ideaId, string ideaTitle, Guid submittedByUserId)
        {
            var activeUsers = await _dbContext.Users
                .Where(u => u.Status == UserStatus.Active && u.UserId != submittedByUserId)
                .Select(u => u.UserId)
                .ToListAsync();

            var notifications = activeUsers.Select(uid => new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = uid,
                Type = NotificationType.NewIdea,
                Message = $"New idea submitted: {ideaTitle}",
                Status = NotificationStatus.Unread,
                CreatedDate = DateTime.UtcNow,
                IdeaId = ideaId
            });

            _dbContext.Notifications.AddRange(notifications);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateManagerDecisionNotificationAsync(Guid ideaId, string ideaTitle, Guid submittedByUserId, Guid reviewerId, string reviewerName, string decision)
        {
            try
            {
                var notification = new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = submittedByUserId,
                    Type = NotificationType.ReviewDecision,
                    Message = $"Your idea \"{ideaTitle}\" has been {decision.ToLower()} by {reviewerName}",
                    Status = NotificationStatus.Unread,
                    CreatedDate = DateTime.UtcNow,
                    IdeaId = ideaId,
                    ReviewerId = reviewerId
                };

                _dbContext.Notifications.Add(notification);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating manager decision notification: {ex.Message}");
            }
        }
    }
}