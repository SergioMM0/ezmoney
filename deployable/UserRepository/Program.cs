using Messages.RPC;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Monitoring;
using OpenTelemetry.Trace;
using Polly;
using Polly.Extensions.Http;
using RPC;
using RPC.RpcFactory;
using UserRepository.Repository;
using UserRepository.Service;

var builder = WebApplication.CreateBuilder(args);

/*** START OF TRACING CONFIGURATION ***/
var serviceName = "UserRespository";
var serviceVersion = "1.0.0";

builder.Services.AddOpenTelemetry().Setup(serviceName, serviceVersion);
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));
/*** END OF TRACING CONFIGURATION ***/

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.Configure<Topics>(builder.Configuration.GetSection("RPCMessages"));
builder.Services.AddSingleton<Topics>(sp =>
    sp.GetRequiredService<IOptions<Topics>>().Value);
builder.Services.AddSingleton<IConnectionFactoryProvider, RpcFactory>();

/*
builder.Services.AddDbContext<UserRepositoryContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("UserDbConnectionString"), new MySqlServerVersion(new Version(5, 7))));
*/

builder.Services.AddDbContext<UserRepositoryContext>(options =>
    options.UseSqlite("Data source=./db.db"));

builder.Services.AddScoped<IUserRepository, UserRepository.Repository.UserRepository>();
builder.Services.AddScoped<UserRepositoryService>();
builder.Services.AddScoped<UserRepositoryHandlers>();
Thread thread = new Thread(() => builder.Services.AddHostedService<RpcBackgroundService>());
thread.Start();



var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();


app.Run();
