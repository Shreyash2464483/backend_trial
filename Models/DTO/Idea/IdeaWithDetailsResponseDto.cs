using backend_trial.Models.DTO.Comment;
using backend_trial.Models.DTO.Review;

namespace backend_trial.Models.DTO.Idea
{
    public class IdeaWithDetailsResponseDto
    {
        public Guid IdeaId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public Guid SubmittedByUserId { get; set; }
        public string SubmittedByUserName { get; set; } = null!;
        public DateTime SubmittedDate { get; set; }
        public string Status { get; set; } = null!;
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public Guid? ReviewedByUserId { get; set; }
        public string? ReviewedByUserName { get; set; }
        public string? ReviewComment { get; set; }
        public List<CommentResponseDto> Comments { get; set; } = new List<CommentResponseDto>();
        public List<ReviewResponseDto> Reviews { get; set; } = new List<ReviewResponseDto>();
    }
}
