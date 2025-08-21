using CRMBackend;
using CRMBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Postgres config
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection")
                        ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddScoped<EmployeeServices>();
builder.Services.AddScoped<PlayerServices>();
builder.Services.AddScoped<GameServices>();
builder.Services.AddScoped<CampaignServices>();
builder.Services.AddScoped<FeedbackServices>();
builder.Services.AddScoped<PlayerGameServices>();
builder.Services.AddScoped<NotificationServices>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
        throw;
    }
});


app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
