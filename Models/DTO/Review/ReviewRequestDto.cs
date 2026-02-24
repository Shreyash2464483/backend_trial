namespace backend_trial.Models.DTO.Review
{
    public class ReviewRequestDto
    {
        public string Feedback { get; set; } = null!;
        public string Decision { get; set; } = null!; // "Approve" or "Reject"
    }
}
