using backend_trial.Models.Domain;

namespace backend_trial.Models.DTO
{
    public class AuthResponseDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public string Token { get; set; } = null!;
    }
}
