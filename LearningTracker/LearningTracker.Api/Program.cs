using FluentValidation;
using LearningTracker.Behaviors;
using LearningTracker.Configurations;
using LearningTracker.Database;
using LearningTracker.Services;
using LearningTracker.Services.Base;
using LearningTracker.Services.TextExtractors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace LearningTracker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddDbContext<LearningTrackerDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("LearningTracker.Api")));

        var assembly = typeof(Program).Assembly;
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        builder.Services.AddValidatorsFromAssembly(assembly);

        builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Auth"));

        builder.Services.AddAuthentication(o => {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            var authOptions = builder.Configuration.GetSection("Auth").Get<AuthOptions>()!;
            options.Authority = authOptions.Issuer;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = authOptions.Issuer,
                ValidAudience = authOptions.Audience,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(authOptions.Key))
            };
            options.Configuration = new OpenIdConnectConfiguration();
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
        builder.Services.AddScoped<ITokenProvider, TokenProvider>();
        builder.Services.AddScoped<TextExtractorService>();
        builder.Services.AddScoped<ITextExtractor, PdfTextExtractor>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
