using AuthService;
using AuthService.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SecretKey is now stores in appsettings.json, and is injected into JWTTokenService
// This may be moved into a more secure location in the future
builder.Services.Configure<AuthServiceConfiguration>(builder.Configuration.GetSection("Configuration"));
builder.Services.AddSingleton<AuthServiceConfiguration>(sp => sp.GetRequiredService<IOptions<AuthServiceConfiguration>>().Value);

builder.Services.AddSingleton<JWTTokenService>();

builder.Services.AddSingleton<AuthService.Services.AuthService>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
