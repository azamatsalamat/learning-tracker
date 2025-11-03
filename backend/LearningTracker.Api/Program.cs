using FluentValidation;
using LearningTracker.Behaviors;
using LearningTracker.Configurations;
using LearningTracker.Database;
using LearningTracker.Services;
using LearningTracker.Services.Base;
using LearningTracker.Services.ResumeParsers;
using LearningTracker.Services.TextExtractors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace LearningTracker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddDbContext<LearningTrackerDbContext>(options =>
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(
                builder.Configuration.GetConnectionString("DefaultConnection"));
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            options.UseNpgsql(
                dataSource,
                b => b.MigrationsAssembly("LearningTracker.Api"));
        });

        var assembly = typeof(Program).Assembly;
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        builder.Services.AddValidatorsFromAssembly(assembly);

        builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Auth"));
        builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("Application"));

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
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below."
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        builder.Services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
        builder.Services.AddScoped<ITokenProvider, TokenProvider>();
        builder.Services.AddScoped<TextExtractorService>();
        builder.Services.AddScoped<ITextExtractor, PdfTextExtractor>();
        builder.Services.AddScoped<IResumeParser, BasicResumeParser>();

        var app = builder.Build();

        var applicationOptions = builder.Configuration.GetSection("Application").Get<ApplicationOptions>()!;
        if (applicationOptions.CorsOrigins.Any()) {
            app.UseCors(x =>
                x.WithOrigins(applicationOptions.CorsOrigins)
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
