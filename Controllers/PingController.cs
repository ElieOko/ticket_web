using Microsoft.AspNetCore.Mvc;

namespace SCustomers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        public IActionResult OnGet()
        {
            return Ok();
        }
    }
}
