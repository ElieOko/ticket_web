using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCustomers.Dtos;
using SCustomers.Models;
using SCustomers.Services;

namespace SCustomers.Controllers
{
    [Authorize(Roles ="admin")]
    public class UsersController : BaseController
    {
        public UsersController(IUserService service)
        {
            UserService = service;
        }
        public IUserService UserService { get; }

        public async Task<IActionResult> OnGetUsers([FromQuery]UserParam userParam, CancellationToken cancellationToken)
        {
            return Ok(await UserService.GetUsers(userParam,cancellationToken));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserHandler(int id, CancellationToken cancellationToken)
        {
            return Ok(await UserService.GetUserAsync(id, cancellationToken));
        }

        [HttpPost("checkusername")]
        public async Task<IActionResult> CheckUsernameHandler(CheckUsernameDto.Request request, 
            CancellationToken cancellationToken)
        {
            return Ok(await UserService.CheckUsernameAsync(request, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> PostUserHandler(UserCreateDto request, CancellationToken cancellationToken)
        {
            return Ok(new { UserId = await UserService.CreateUserAsync(request, cancellationToken) });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserHandler(int id, UserUpdateDto request, CancellationToken cancellationToken)
        {
            await UserService.UpdateUserAsync(id, request, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserHandler(int id, CancellationToken cancellationToken)
        {
            await UserService.DeleteUserAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/resetpassword")]
        public async Task<IActionResult> ResetUserPasswordHandler(int id, UserResetPasswordDto requset,
            CancellationToken cancellationToken)
        {
            await UserService.ResetUserPasswordAsync(id, requset, cancellationToken);
            return NoContent();
        }

        [HttpPut("update")]
        public async Task<IActionResult> OnPutUsers(List<UserGridUpdateDto> request, CancellationToken cancellationToken)
        {
            foreach (var user in request)
                await UserService.UpdateUserGridAsync(user, cancellationToken);
            return NoContent();
        }

        [HttpPost("delete")]
        public async Task<IActionResult> OnDeleteUsers(List<UserGridUpdateDto> request, CancellationToken cancellationToken)
        {
            foreach (var user in request)
                await UserService.DeleteGridUserAsync(user.UserId, cancellationToken);
            return NoContent();
        }

        [HttpPut("unlock")]
        public async Task<IActionResult> OnPutUnLock(UserUnlockDto request, CancellationToken cancellationToken)
        {
            await UserService.UnLockUserAsync(request, cancellationToken);
            return NoContent();
        }
    }
}
