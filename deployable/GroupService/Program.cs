using Messages.RPC;
using Microsoft.Extensions.Options;
using RPC;
using RPC.RpcFactory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


// Configure topics
builder.Services.AddSingleton<IConnectionFactoryProvider, RpcFactory>();
builder.Services.Configure<Topics>(builder.Configuration.GetSection("RPCMessages"));
builder.Services.AddSingleton<Topics>(sp =>
    sp.GetRequiredService<IOptions<Topics>>().Value);
builder.Services.AddSingleton<RpcClient>(provider =>
    new RpcClient(
        provider.GetRequiredService<IOptions<Topics>>().Value.Topic,
        provider.GetRequiredService<IConnectionFactoryProvider>()
    )
);

var app = builder.Build();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
