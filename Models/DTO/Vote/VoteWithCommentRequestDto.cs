namespace backend_trial.Models.DTO.Vote
{
    public class VoteWithCommentRequestDto
    {
        public string VoteType { get; set; } = null!; // "Downvote"
        public string CommentText { get; set; } = null!; // Mandatory for downvote
    }
}
