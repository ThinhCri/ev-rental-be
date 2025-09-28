using Microsoft.AspNetCore.Mvc;

namespace RentalCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(new[] { "User A", "User B", "User C" });
        }
    }
}
