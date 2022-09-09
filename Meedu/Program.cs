using FluentValidation;
using FluentValidation.AspNetCore;
using Meedu;
using Meedu.Entities;
using Meedu.Models;
using Meedu.Models.Validators;
using Meedu.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MeeduConnectionString");
builder.Services.AddDbContext<MeeduDbContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddControllers().AddFluentValidation();

// Authentication

var authSettings = new AuthSettings();

var tokenValidationParams = new TokenValidationParameters()
{
    ValidIssuer = authSettings.JwtIssuer,
    ValidAudience = authSettings.JwtIssuer,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.JwtKey))
};

builder.Configuration.GetSection("Authentication").Bind(authSettings);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = "Bearer";
    x.DefaultScheme = "Bearer";
    x.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(config =>
{
    config.SaveToken = true;
    config.RequireHttpsMetadata = false;
    config.TokenValidationParameters = tokenValidationParams;
});

// Add services to the container.
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
