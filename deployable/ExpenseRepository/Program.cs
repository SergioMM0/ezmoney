using ExpenseRepository.Repository;
using ExpenseRepository.Service;
using Messages.RPC;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Monitoring;
using OpenTelemetry.Trace;
using RPC.RpcFactory;

var builder = WebApplication.CreateBuilder(args);
/* Tracer config **/
var serviceName = "ExpenseRepository";
var serviceVersion = "1.0.0";


builder.Services.AddOpenTelemetry().Setup(serviceName, serviceVersion);
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));
/* End tracer config */

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// Configure topics
builder.Services.Configure<Topics>(builder.Configuration.GetSection("RPCMessages"));
builder.Services.AddSingleton<Topics>(sp =>
    sp.GetRequiredService<IOptions<Topics>>().Value);
builder.Services.AddSingleton<IConnectionFactoryProvider, RpcFactory>();
builder.Services.AddDbContext<ExpenseRepositoryContext>(options =>
    options.UseSqlite("Data source=./db.db"));
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository.Repository.ExpenseRepository>();
builder.Services.AddScoped<ExpenseRepositoryService>();
builder.Services.AddScoped<ExpenseRepositoryHandlers>();
Thread thread = new Thread(() => builder.Services.AddHostedService<RpcBackgroundService>());
thread.Start();


var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.MapControllers();
//app.UseHttpsRedirection();


app.Run();


