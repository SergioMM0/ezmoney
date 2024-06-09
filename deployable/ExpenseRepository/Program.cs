using ExpenseRepository.Repository;
using ExpenseRepository.Service;
using Messages.RPC;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPC.RpcFactory;

var builder = WebApplication.CreateBuilder(args);

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
app.Run();


