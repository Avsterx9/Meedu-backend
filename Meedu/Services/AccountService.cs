using Meedu.Entities;
using Meedu.Exceptions;
using Meedu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Meedu.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
        string GenerateJwtToken(LoginUserDto loginDto);
        Task<UserInfoDto> GetUserInfo();
    }

    public class AccountService : IAccountService
    {
        private readonly MeeduDbContext dbContext;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly ILogger<AccountService> logger;
        private readonly AuthSettings authSettings;
        private readonly IUserContextService userContextService;

        public AccountService(MeeduDbContext dbContext, 
            IPasswordHasher<User> passwordHasher, 
            ILogger<AccountService> logger, 
            AuthSettings authSettings, 
            IUserContextService userContextService)
        {
            this.dbContext = dbContext;
            this.passwordHasher = passwordHasher;
            this.logger = logger;
            this.authSettings = authSettings;
            this.userContextService = userContextService;
        }

        public void RegisterUser(RegisterUserDto dto)
        {
            var newUser = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                RoleId = dto.RoleId,
            };

            var hashedPassword = passwordHasher.HashPassword(newUser, dto.Password);
            newUser.PasswordHash = hashedPassword;

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();
        }

        public string GenerateJwtToken(LoginUserDto loginDto)
        {
            var user = dbContext.Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Email == loginDto.Email);

            if (user is null)
            {
                throw new BadRequestException("Invalid username or password");
            }

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid username or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Name, $"{user.Role.Name}"),
                new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("dd-MM-yyyy"))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(authSettings.JwtExpireDays);

            var token = new JwtSecurityToken(authSettings.JwtIssuer,
                authSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public async Task<UserInfoDto> GetUserInfo()
        {
            var userId = userContextService.GetUserId;

            if(userId is null)
            {
                throw new BadRequestException("User does not exist");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            return new UserInfoDto()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                DateOfBirth = user.DateOfBirth,
                LastName = user.LastName,
                RoleId = user.RoleId
            };
        }
    }
}
