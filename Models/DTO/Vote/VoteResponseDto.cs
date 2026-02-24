namespace backend_trial.Models.DTO.Vote
{
    public class VoteResponseDto
    {
        public Guid VoteId { get; set; }
        public Guid IdeaId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string VoteType { get; set; } = null!;
    }
}
