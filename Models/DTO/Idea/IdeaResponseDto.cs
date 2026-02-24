using backend_trial.Models.DTO.Comment;

namespace backend_trial.Models.DTO.Idea
{
    public class IdeaResponseDto
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
        public List<CommentResponseDto> Comments { get; set; } = new List<CommentResponseDto>();
    }
}
