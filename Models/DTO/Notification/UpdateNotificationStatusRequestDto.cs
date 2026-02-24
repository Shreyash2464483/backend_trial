namespace backend_trial.Models.DTO.Notification
{
    public class UpdateNotificationStatusRequestDto
    {
        public Guid NotificationId { get; set; }
        public string Status { get; set; } = null!;
    }
}