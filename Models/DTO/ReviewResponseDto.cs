namespace backend_trial.Models.DTO
{
    public class ReviewResponseDto
    {
        public Guid ReviewId { get; set; }
        public Guid IdeaId { get; set; }
        public Guid ReviewerId { get; set; }
        public string ReviewerName { get; set; } = null!;
        public string Feedback { get; set; } = null!;
        public DateTime ReviewDate { get; set; }
    }
}
