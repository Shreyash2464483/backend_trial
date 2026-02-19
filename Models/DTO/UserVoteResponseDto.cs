namespace backend_trial.Models.DTO
{
    public class UserVoteResponseDto
    {
        public bool HasVoted { get; set; }
        public string? VoteType { get; set; }
    }
}