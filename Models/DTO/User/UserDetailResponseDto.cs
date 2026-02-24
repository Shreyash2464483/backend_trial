namespace backend_trial.Models.DTO.User
{
    public class UserDetailResponseDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int IdeasSubmitted { get; set; }
        public int CommentsPosted { get; set; }
        public int VotesCasted { get; set; }
        public int ReviewsSubmitted { get; set; }
    }
}
