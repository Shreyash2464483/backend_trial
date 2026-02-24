using System.ComponentModel.DataAnnotations;

namespace backend_trial.Models.DTO.Auth
{
    public class RegisterRequestDto
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [RegularExpression("^(Employee|Manager|Admin)$", ErrorMessage = "Role must be 'Employee', 'Manager', or 'Admin'")]
        public string Role { get; set; }
    }
}
