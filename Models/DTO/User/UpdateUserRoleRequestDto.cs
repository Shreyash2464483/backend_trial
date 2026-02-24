namespace backend_trial.Models.DTO.User
{
    public class UpdateUserRoleRequestDto
    {
        public string Role { get; set; } = null!; // "Employee", "Manager", or "Admin"
    }
}
