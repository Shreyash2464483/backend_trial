using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    // Only accessible to authenticated users
    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        return Ok(new { Message = "This is a protected endpoint" });
    }

    // Only accessible to Admins
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminOnly()
    {
        return Ok(new { Message = "Admin only endpoint" });
    }

    // Accessible to Managers and Admins
    [HttpGet("management")]
    [Authorize(Roles = "Manager,Admin")]
    public IActionResult ManagementOnly()
    {
        return Ok(new { Message = "Management endpoint" });
    }
}
