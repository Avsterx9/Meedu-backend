using Meedu.Entities;
using Meedu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Meedu.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
        string GenerateJwtToken(LoginUserDto loginDto);
    }

    public class AccountService : IAccountService
    {
        private readonly MeeduDbContext dbContext;
        private readonly IPasswordHasher<User> passwordHasher;

        public AccountService(MeeduDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            this.dbContext = dbContext;
            this.passwordHasher = passwordHasher;
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

            if(user is null)
            {
                Console.WriteLine("IMPLEMENT");
            }
            //if (!user)
            //{
            //    throw new BadRequestException("Invalid username or password");
            //}

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if(result == PasswordVerificationResult.Failed)
            {
                //    throw new BadRequestException("Invalid username or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Name, $"{user.Role.Name}"),
            };
            return String.Empty;
        }
    }
}
