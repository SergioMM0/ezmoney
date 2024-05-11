using GroupRepository.Repository;
using GroupRepository.Service;
using Messages.RPC;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPC.RpcFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure topics
builder.Services.Configure<Topics>(builder.Configuration.GetSection("RPCMessages"));
builder.Services.AddSingleton<Topics>(sp =>
    sp.GetRequiredService<IOptions<Topics>>().Value);
builder.Services.AddSingleton<IConnectionFactoryProvider, RpcFactory>();
builder.Services.AddDbContext<GroupRepositoryContext>(options =>
    options.UseSqlite("Data source=./db.db"));
builder.Services.AddScoped<IGroupRepository, GroupRepository.Repository.GroupRepository>();
builder.Services.AddScoped<GroupRepositoryService>();
builder.Services.AddScoped<GroupRepositoryHandlers>();
builder.Services.AddHostedService<RpcBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.Run();


