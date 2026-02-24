namespace backend_trial.Models.DTO.Review
{
    public class ReviewSubmitDto
    {
        public Guid IdeaId { get; set; }
        public string Feedback { get; set; } = null!;
        public string Decision { get; set; } = null!; // "Approve" or "Reject"
        public string? RejectionReason { get; set; } // Mandatory for rejections only
    }
}