namespace backend_trial.Models.DTO
{
    public class ReviewDecisionDto
    {
        public Guid IdeaId { get; set; }
        public string Decision { get; set; } = null!; // "Approve" or "Reject"
        public string? RejectionReason { get; set; } // Mandatory only when Decision == "Reject"
    }
}