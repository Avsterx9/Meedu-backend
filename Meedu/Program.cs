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
using NLog.Web;
using Meedu.Middleware;

var builder = WebApplication.CreateBuilder(args);
//builder.Logging.ClearProviders();
builder.Host.UseNLog();

var connectionString = builder.Configuration.GetConnectionString("MeeduConnectionString");
builder.Services.AddDbContext<MeeduDbContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddControllers().AddFluentValidation();

// Authentication

var authSettings = new AuthSettings();

builder.Configuration.GetSection("Authentication").Bind(authSettings);

builder.Services.AddSingleton(authSettings);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = "Bearer";
    x.DefaultScheme = "Bearer";
    x.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(config =>
{
    config.SaveToken = true;
    config.RequireHttpsMetadata = false;
    config.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = authSettings.JwtIssuer,
        ValidAudience = authSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.JwtKey))
    };
});

// SERVICES
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPrivateLessonService, PrivateLessonService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// VALIDATORS
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<PrivateLessonOfferDto>, PrivateLessonOfferDtoValidator>();

builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => options.AddPolicy(name: "MeeduFrontend",
    policy => 
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
    }));

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MeeduFrontend");

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
