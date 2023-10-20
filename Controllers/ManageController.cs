using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCustomers.Dtos;
using SCustomers.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SCustomers.Controllers
{
    [Authorize(Roles ="user, mananger, admin")]
    public class ManageController : BaseController
    {
        public ManageController(IUserService userService)
        {
            UserService = userService;
        }
        public IUserService UserService { get; }

        [HttpGet("userinfo")]
        public async Task<IActionResult> GetUserInfoHandler(CancellationToken cancellationToken)
        {
            return Ok(await UserService.GetCurrentUserInfoAsync(UserId, cancellationToken));
        }

        [HttpPost("profile")]
        public async Task<IActionResult> PostUserProfileHandler(UserUpdateProfileDto request, 
            CancellationToken cancellationToken)
        {
            await UserService.UpdateUserProfileAsync(UserId, request, cancellationToken);
            return NoContent();
        }

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePasswordHandler(UserChangePasswordDto request, 
            CancellationToken cancellationToken)
        {
            await UserService.ChangePasswordAsync(UserId, request, cancellationToken);
            return NoContent();
        }
    }
}
