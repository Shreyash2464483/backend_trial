using backend_trial.Data;
using backend_trial.Models.Domain;
using backend_trial.Models.DTO;
using backend_trial.Repositories;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_trial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IdeaBoardDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ITokenRepository tokenRepository;

        public AuthController(IdeaBoardDbContext context , IConfiguration configuration , ITokenRepository tokenRepository)
        {
            _context = context;
            _configuration = configuration;
            this.tokenRepository = tokenRepository;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            // Check if user with the same email already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("User with this email already exists.");
            }

            // Parse role string to enum
            if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
            {
                return BadRequest("Invalid role. Must be 'Employee', 'Manager', or 'Admin'.");
            }

            // Hash the password before storing
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create new User
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = userRole,
                Status = UserStatus.Active
            };

            // Store user in DataBase
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

           
            return Ok("Registration successful. Please login to continue.");
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Verify password
            if(!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }

            // Check if user is active
            if(user.Status != UserStatus.Active)
            {
                return Unauthorized("User account is not active.");
            }

            // Generate Jwt token
            var token = tokenRepository.CreateJwtToken(user);

            var response = new AuthResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Status = user.Status,
                Token = token
            };
            return Ok(response.Token);
        }
    }
}
