using LearningTracker.Application;
using LearningTracker.Infrastructure;
using LearningTracker.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;

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
        builder.Services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));

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