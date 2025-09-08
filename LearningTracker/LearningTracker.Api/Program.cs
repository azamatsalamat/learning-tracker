using LearningTracker.Api.Configurations;
using LearningTracker.Application;
using LearningTracker.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace LearningTracker.Api;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddApplication();

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

        builder.Services.AddPresentation(builder.Configuration);

        var app = builder.Build();
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
