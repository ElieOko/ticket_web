using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SCustomers.Data;
using SCustomers.Dtos;
using SCustomers.Entities;
using SCustomers.Exceptions;
using SCustomers.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCustomers.Services
{
    public interface IUserService
    {
        User GetById(int id);
        Task<CheckUsernameDto.Response> CheckUsernameAsync(CheckUsernameDto.Request request, CancellationToken cancellationToken);
        Task<PagedList<UserDto>> GetUsers(UserParam queryParams, CancellationToken cancellationToken);
        Task<UserDto> GetUserAsync(int userId, CancellationToken cancellationToken);
        Task<int> CreateUserAsync(UserCreateDto request, CancellationToken cancellationToken);
        Task UnLockUserAsync(UserUnlockDto request, CancellationToken cancellationToken);
        Task UpdateUserAsync(int userId, UserUpdateDto request, CancellationToken cancellationToken);
        Task DeleteUserAsync(int userId, CancellationToken cancellationToken);
        Task ResetUserPasswordAsync(int userId, UserResetPasswordDto request, CancellationToken cancellationToken);
        Task UpdateUserGridAsync(UserGridUpdateDto request, CancellationToken cancellationToken);
        Task DeleteGridUserAsync(int userId, CancellationToken cancellationToken);
        Task<AuthResponse> SignInAsync(AuthRequest request, CancellationToken cancellationToken);
        Task<UserManageDto> GetCurrentUserInfoAsync(int userId, CancellationToken cancellationToken);
        Task UpdateUserProfileAsync(int userId, UserUpdateProfileDto request,
            CancellationToken cancellationToken);
        Task ChangePasswordAsync(int userId, UserChangePasswordDto request, CancellationToken cancellationToken);
    }
    public class UserService:IUserService
    {
        public UserService(AppDbContext context, IOptions<JwtOptions> options, 
            IHttpContextAccessor accessor,
            IOptions<AccountOptions> accountOptions)
        {
            DbContext = context;
            Jwt = options.Value;
            ContextAccessor = accessor;
            AccountOptions = accountOptions.Value;
        }
        private AppDbContext DbContext { get; set; }
        private JwtOptions Jwt { get; }
        private AccountOptions AccountOptions { get; }
        private IHttpContextAccessor ContextAccessor { get; set; }
        public async Task<CheckUsernameDto.Response> CheckUsernameAsync(CheckUsernameDto.Request request, 
            CancellationToken cancellationToken)
        {
            var username = request.Username.ToUpper();
            var userExists = await DbContext.Users.AsNoTracking()
                                        .Where(x => x.UserName.ToUpper().Equals(username)).AnyAsync(cancellationToken);
            return new CheckUsernameDto.Response { IsTaken = userExists };
        }
        public async Task<int> CreateUserAsync(UserCreateDto request, CancellationToken cancellationToken)
        {
            var normalizedUsername = request.UserName.ToUpper();
            var userExists = await DbContext.Users.AsNoTracking()
                                        .Where(x => x.UserName.ToUpper().Equals(normalizedUsername))
                                        .AnyAsync(cancellationToken);

            if (userExists)
                throw new BadOperationException("Il existe un utilisateur avec cet identifiant");

            var user = new User
            {
                UserName = request.UserName,
                Password = CreatePasswordHash(request.Password),
                FullName = request.FullName,
                MinAmount = request.MinAmount,
                MaxAmount = request.MaxAmount,
                MaxAttempt = AccountOptions.MaxAttempt,
                AccessFailedCount = 0,
                AccessLevel = request.AccessLevel,
                Locked = request.Locked,
                BranchId = request.Branch,
                DateCreated = DateTime.Now
            };


            if(request.Permissions != null)
            {
                user.UserPermissions = new List<UserPermission>();
                foreach (var type in request.Permissions)
                {
                    var transferType = await DbContext.TransferTypes.FindAsync(type);
                    
                    if(transferType != null)
                    {
                        user.UserPermissions.Add(new UserPermission
                        {
                            TransferTypeId = transferType.TransferTypeId
                        });
                    }
                }
            }

            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync(cancellationToken);
            return user.UserId;
        }
        public async Task UnLockUserAsync(UserUnlockDto request, CancellationToken cancellationToken)
        {
            var user = await DbContext.Users.Where(x => x.UserId == request.UserId)
                                        .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new NotFoundException($"Aucun utilisateur avec id {request.UserId}");

            user.Locked = false;
            user.AccessFailedCount = 0;

            await DbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task<PagedList<UserDto>> GetUsers(UserParam userParam, CancellationToken cancellationToken)
        {
            var users = DbContext.Users.Include(x => x.Branch).AsNoTracking();

            if (!string.IsNullOrEmpty(userParam.Query))
            {
                var term = userParam.Query.ToLower();
                users = users.Where(x => x.UserName.ToLower().Contains(term) 
                || x.FullName.ToLower().Contains(term) || x.Branch.BranchName.ToLower().Contains(term));
            }

            var count = await users.CountAsync();

            var data = await users.Select(x => new UserDto
            {
                UserId = x.UserId,
                UserName = x.UserName,
                FullName = x.FullName,
                Branch = x.Branch,
                AccessLevel = x.AccessLevel,
                Locked = x.Locked
            }).OrderByDescending(x => x.UserId).Skip((userParam.PageNumber - 1) * userParam.PageSize)
                                        .Take(userParam.PageSize)
                                        .ToListAsync(cancellationToken);

            
            return new PagedList<UserDto>(data, userParam.Query, count, userParam.PageNumber, userParam.PageSize);
        }
        public async Task<UserDto> GetUserAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await DbContext.Users
                                            .Include(x => x.UserPermissions)
                                            .AsNoTracking()
                                            .Where(x => x.UserId.Equals(userId))
                                            .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new NotFoundException($"No user found with id {userId}");

            return new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                FullName = user.FullName,
                AccessLevel = user.AccessLevel,
                BranchId = user.BranchId,
                Permissions = user.UserPermissions.Select(x => x.TransferTypeId).ToList()
            };

        }
        public async Task UpdateUserAsync(int userId, UserUpdateDto request, CancellationToken cancellationToken)
        {
            if (!userId.Equals(request.UserId))
                throw new BadOperationException("User Id is required");

            var user = await DbContext.Users
                .Include(x => x.UserPermissions)
                .Where(x => x.UserId.Equals(request.UserId)).FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new NotFoundException($"No user found with id {request.UserId}");

            if (!string.IsNullOrEmpty(request.FullName) 
                && user.FullName.ToUpper().Equals(request.FullName.ToUpper()))
                user.FullName = request.FullName;

            if (!user.AccessLevel.Equals(request.AccessLevel))
                user.AccessLevel = request.AccessLevel;

            if(!user.BranchId.Equals(request.Branch))
                user.BranchId = request.Branch;



            if(request.Permissions != null)
            {
                var selectedTypesHS = new HashSet<int>(request.Permissions.ToList());
                var userTypes = new HashSet<int>(user.UserPermissions.Select(x => x.TransferTypeId).ToList());

                foreach (var type in DbContext.TransferTypes)
                {
                    if (selectedTypesHS.Contains(type.TransferTypeId))
                    {
                        if (!userTypes.Contains(type.TransferTypeId))
                        {
                            user.UserPermissions.Add(new UserPermission { TransferTypeId = type.TransferTypeId });
                        }
                    }
                    else
                    {
                        if (userTypes.Contains(type.TransferTypeId))
                        {
                            var useType = user.UserPermissions.FirstOrDefault(x => x.TransferTypeId == type.TransferTypeId);
                            user.UserPermissions.Remove(useType);
                        }
                    }
                }
            }
            else
            {
                user.UserPermissions = new List<UserPermission>();
            }

            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteUserAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await DbContext.Users
                .Include(x => x.UserPermissions)
                .Where(x => x.UserId.Equals(userId)).FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new NotFoundException($"No user found with id {userId}");

            DbContext.Users.Remove(user);
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateUserGridAsync(UserGridUpdateDto request, CancellationToken cancellationToken)
        {
            var user = await DbContext.Users.Where(x => x.UserId == request.UserId)
                                        .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return;

            user.UserName = request.UserName;
            user.FullName = request.FullName;
            user.AccessLevel = request.AccessLevel;
            user.MinAmount = request.MinAmount;
            user.MaxAmount = request.MaxAmount;
            user.BranchId = request.Branch.BranchId;

            if (!string.IsNullOrEmpty(request.Password))
                user.Password = CreatePasswordHash(request.Password);
            if(!request.Locked && user.Locked)
            {
                user.Locked = false;
                user.AccessFailedCount = 0;
            }
            else
            {
                user.Locked = request.Locked;
            }

            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteGridUserAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await DbContext.Users.Where(x => x.UserId == userId)
                                        .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return;

            DbContext.Users.Remove(user);

            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task ResetUserPasswordAsync(int userId, UserResetPasswordDto request, CancellationToken cancellationToken)
        {
            if (!userId.Equals(request.UserId))
                throw new BadOperationException("The user Key in the route does not match the user in the body");

            var user = await DbContext.Users.Where(x => x.UserId.Equals(request.UserId)).FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new NotFoundException($"No user found with id {request.UserId}");

            user.Password = CreatePasswordHash(request.Password);
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        private string CreatePasswordHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        public async Task<AuthResponse> SignInAsync(AuthRequest request, CancellationToken cancellationToken)
        {
            var deviceType = "web";
            // Get the user device type from the request header
            if (ContextAccessor.HttpContext.Request.Headers.TryGetValue("ClientType", out var device))
            {
                deviceType = device.ToString();
            }

            //Get the user remote ip address
            var remoteIpAddress = ContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            //Trying to retrieve the user record from the database given the provided username
            var user = await DbContext.Users.Include(x => x.Branch)
                                            .Where(x => x.UserName == request.Username)
                                            .FirstOrDefaultAsync(cancellationToken);
            //if the record does not exist, throw the exception
            if (user == null)
                throw new Exceptions.UnauthorizedAccessException(AppConstants.LoginFailedReasonPwd, AppConstants.WrongUsernameErrorCode);
            //if the record exists, but it is locked, throw the exception too
            if (user.Locked)
            {
                DbContext.UserLogs.Add(new UserLog
                {
                    UserId = user.UserId,
                    ClientType = deviceType,
                    LogTime = DateTime.Now,
                    AccessType = AppConstants.FailedLoginType,
                    ClientIpAddress = remoteIpAddress
                });

                //save all changes before leaving
                await DbContext.SaveChangesAsync(cancellationToken);

                throw new Exceptions.UnauthorizedAccessException(AppConstants.LoginFailedReasonLocked, AppConstants.LockedAccountErrorCode);
            }

            //Now compare the stored hashed password and the one provided by the user
            //if no match, throw the exception
            if (!VerifyPassword(request.Password, user.Password))
            {
                //increment failed counter
                user.AccessFailedCount++;
                //lock the account if reached max attempt
                if (user.AccessFailedCount == user.MaxAttempt)
                    user.Locked = true;
                //TODO: Log the auth here
                DbContext.UserLogs.Add(new UserLog
                {
                    UserId = user.UserId,
                    ClientType = deviceType,
                    LogTime = DateTime.Now,
                    AccessType = AppConstants.FailedLoginType,
                    ClientIpAddress = remoteIpAddress
                });
                //save all changes before leaving
                await DbContext.SaveChangesAsync(cancellationToken);

                throw new Exceptions.UnauthorizedAccessException(AppConstants.LoginFailedReasonPwd, AppConstants.WrongPasswordErrorCode);
            }

            //reset the failed counter
            user.AccessFailedCount = 0;
            //TO DO: Log the auth here:
            DbContext.UserLogs.Add(new UserLog
            {
                UserId = user.UserId,
                ClientType = deviceType,
                LogTime = DateTime.Now,
                AccessType = AppConstants.SuccessLoginType,
                ClientIpAddress = remoteIpAddress
            });
            //save all pending changes before leaving
            await DbContext.SaveChangesAsync(cancellationToken);

            return new AuthResponse
            {
                UserId = user.UserId,
                Username = user.UserName,
                AccessToken = GenerateToken(user),
                BranchId = user.BranchId,
                IsAdmin = user.AccessLevel == Acl.Admin,
                UserType = user.UserType,
                CanManage = user.AccessLevel == Acl.Admin || user.AccessLevel == Acl.Manager,
                Branch = user.Branch
            };
        }
        public User GetById(int id)
        {
            return DbContext.Users.FirstOrDefault(x => x.UserId == id);
        }
        private string GenerateToken(User user)
        {
            // generate token that is valid for 9 hours
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Jwt.SigningKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim("bid", user.BranchId.ToString())
                }),
                Expires = DateTime.Now.AddHours(9),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            if(user.AccessLevel == Acl.Admin)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, "admin"));
            }
            else if (user.AccessLevel == Acl.Manager)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, "manager"));
            }
            else
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, "user"));
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task ChangePasswordAsync(int userId, UserChangePasswordDto request, CancellationToken cancellationToken)
        {
            var user = await DbContext.Users.Where(x => x.UserId.Equals(userId)).FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new NotFoundException($"No user found with id {userId}");

            if (!VerifyPassword(request.CurrentPassword, user.Password))
                throw new BadOperationException("L'ancien mot de passe fourni est incorrect");

            user.Password = CreatePasswordHash(request.NewPassword);

            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateUserProfileAsync(int userId, UserUpdateProfileDto request, 
            CancellationToken cancellationToken)
        {
            var user = await DbContext.Users.Where(x => x.UserId.Equals(userId)).FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new NotFoundException($"No user found with id {userId}");

            if (!user.UserName.ToUpper().Equals(request.UserName.ToUpper()))
            {
                var userExists = await DbContext.Users.Where(x => x.UserName.ToUpper().Equals(request.UserName))
                                                        .AnyAsync(cancellationToken);

                if (userExists)
                    throw new BadOperationException($"Il ya déjà un utilisateur nommé {request.UserName}");
                user.UserName = request.UserName;
            }

            if (!user.FullName.ToUpper().Equals(request.FullName.ToUpper()))
                user.FullName = request.FullName;

            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserManageDto> GetCurrentUserInfoAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await DbContext.Users
                .Include(x => x.Branch)
                .AsNoTracking()
                .Where(x => x.UserId.Equals(userId)).FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new NotFoundException($"No user found with id {userId}");

            return new UserManageDto
            {
                UserId = user.UserId,
                Branch = user.Branch.BranchName,
                UserProfile = new UserProfileDto
                {
                    UserName = user.UserName,
                    FullName = user.FullName
                }
            };
        }
    }
}
