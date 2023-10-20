using Microsoft.AspNetCore.Mvc;
using SCustomers.Models;
using SCustomers.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SCustomers.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public AuthController(IUserService service)
        {
            AuthService = service;
        }
        
        public IUserService AuthService { get; }

        [HttpPost("token")]
        public async Task<IActionResult> OnPostLogin(AuthRequest request, CancellationToken  cancellationToken)
        {
            return Ok(await AuthService.SignInAsync(request, cancellationToken));
        }
    }
}
