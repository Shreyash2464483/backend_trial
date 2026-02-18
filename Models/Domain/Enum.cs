namespace backend_trial.Models.Domain
{
    public enum UserRole { Employee, Manager, Admin }
    public enum UserStatus { Active, Inactive }
    public enum IdeaStatus { Rejected, UnderReview, Approved }
    public enum VoteType { Upvote, Downvote }
    public enum ReviewDecision { Approved, Rejected }
    public enum NotificationType { NewIdea, ReviewDecision }
    public enum NotificationStatus { Unread, Read }
}

