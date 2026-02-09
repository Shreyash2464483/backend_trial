using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_trial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdeaController : ControllerBase
    {
        [HttpGet("/getideas")]
        [Authorize(Roles="Manager")]
        public IActionResult GetAllIdeas()
        {
            return Ok();
        }
    }
}
