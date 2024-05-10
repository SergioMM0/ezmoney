using Domain.packages;
using Messages.RPC;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPC.RpcFactory;
using UserRepository.Repository;
using UserRepository.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Topics>(builder.Configuration.GetSection("RPCMessages"));
builder.Services.AddSingleton<Topics>(sp =>
    sp.GetRequiredService<IOptions<Topics>>().Value);
builder.Services.AddSingleton<IConnectionFactoryProvider, RpcFactory>();
// builder.Services.AddDbContext<UserRepositoryContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("UserDbConnectionString"), new MySqlServerVersion(new Version(5, 7))));
builder.Services.AddDbContext<UserRepositoryContext>(options =>
    options.UseSqlite("Data source=./db.db"));
builder.Services.AddScoped<IUserRepository, UserRepository.Repository.UserRepository>();
builder.Services.AddScoped<UserRepositoryService>();
builder.Services.AddScoped<UserRepositoryHandlers>();
builder.Services.AddHostedService<RpcBackgroundService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();



app.Run();


