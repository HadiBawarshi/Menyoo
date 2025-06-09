using Menyoo.API.Middlewares;
using Menyoo.Application.Extensions;
using Menyoo.Infrastructure.Extensions;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;


var logger = LogManager.Setup()
    .LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Remove default logging
    builder.Logging.ClearProviders();
    //In dev use trace
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    //In prod
    //builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);

    builder.Host.UseNLog(); // Register NLog

    // Add services to the container.
    builder.Services.AddControllers();

    // Add Db context and Identity
    builder.Services.AddIdentityDbContext(builder.Configuration);

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddApplicationServices();

    builder.Services.AddAutoMapper(typeof(Program));

    builder.Services.AddInfrastructureIdentityServices();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Menyoo.API", Version = "v1" });
    });

    //JWT Authentication
    builder.Services.AddJWTAuthentication(builder.Configuration);


    var app = builder.Build();

    // Global exception handler middleware
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped due to an exception during startup.");
    throw;
}
finally
{
    LogManager.Shutdown(); // Flush and stop NLog

}