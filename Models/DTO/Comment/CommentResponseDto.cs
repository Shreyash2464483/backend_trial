namespace backend_trial.Models.DTO.Comment
{
    public class CommentResponseDto
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}
