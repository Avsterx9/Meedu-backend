using AutoMapper;
using Meedu.Entities;
using Meedu.Exceptions;
using Meedu.Helpers;
using Meedu.Models;
using Meedu.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Meedu.Services
{
    public class AccountService : IAccountService
    {
        private readonly MeeduDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AccountService> _logger;
        private readonly AuthSettings _authSettings;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public AccountService(MeeduDbContext dbContext, 
            IPasswordHasher<User> passwordHasher, 
            ILogger<AccountService> logger, 
            AuthSettings authSettings, 
            IUserContextService userContextService,
            IMapper mapper)
        {
            _context = dbContext;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _authSettings = authSettings;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public async Task RegisterUserAsync(RegisterUserDto dto)
        {
            var newUser = _mapper.Map<User>(dto);
            var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
            newUser.PasswordHash = hashedPassword;

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GenerateJwtTokenAsync(LoginUserDto loginDto)
        {
            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == loginDto.Email)
                ?? throw new BadRequestException(ExceptionMessages.InvalidUsernameOrPassword);

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

            if (result == PasswordVerificationResult.Failed)
                throw new BadRequestException(ExceptionMessages.InvalidUsernameOrPassword);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Name, $"{user.Role.Name}"),
                new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("dd-MM-yyyy"))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authSettings.JwtIssuer,
                _authSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public async Task<UserInfoDto> GetUserInfoAsync()
        {
            var userId = _userContextService.GetUserIdFromToken();

            var user = await _context.Users
                .Include(x => x.Image)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new NotFoundException(ExceptionMessages.UserNotFound);

            return _mapper.Map<UserInfoDto>(user);
        }

        public async Task<UserInfoDto> UpdateUserDataAsync(UpdateUserDataRequest request)
        {
            var userId = _userContextService.GetUserId;

            var user = await _context.Users
                .Include(x => x.Image)
                .FirstOrDefaultAsync(x => x.Id == userId)
                ?? throw new BadRequestException("User does not exist");

            user.PhoneNumber = request.PhoneNumber;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.DateOfBirth = request.DateOfBirth;

            await _context.SaveChangesAsync();

            return new UserInfoDto
            {
                Id = user.Id,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = user.RoleId
            };
        }

        public async Task SetUserImageAsync(IFormFile file)
        {
            var userId = _userContextService.GetUserId;

            var user = await _context.Users
                .Include(x => x.Image)
                .FirstOrDefaultAsync(x => x.Id == userId)
                ?? throw new BadRequestException("");

            if (file == null || file.Length == 0)
                throw new BadRequestException("No file selected.");

            byte[] data;
            using (var stream = new MemoryStream())
            {
                Bitmap bitmap = new Bitmap(file.OpenReadStream());
                bitmap.Save(stream, ImageFormat.Jpeg);
                data = stream.ToArray();
            }

            var image = new Meedu.Entities.Image
            {
                ContentType = file.ContentType,
                Data = data
            };

            user.Image = image;

            _context.Images.Add(image);
            await _context.SaveChangesAsync();
        }
    }
}
