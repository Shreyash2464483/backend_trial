namespace backend_trial.Models.DTO.Idea
{
    public class ChangeIdeaStatusRequestDto
    {
        public string Status { get; set; } = null!; // "Rejected", "UnderReview", or "Approved"
        public string? ReviewComment { get; set; } // Mandatory when changing status to "Rejected"
    }
}
