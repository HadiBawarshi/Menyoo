using Menyoo.Application.Extensions;
using Menyoo.Infrastructure.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
