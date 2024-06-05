using FeatureHubSDK;
using IO.FeatureHub.SSE.Model;
using Messages.RPC;
using Microsoft.Extensions.Options;
using RPC;
using RPC.RpcFactory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Add HTTP client
builder.Services.AddHttpClient();

// FeatureHub
var featureHubConfig = new EdgeFeatureHubConfig(
    "http://localhost:8085",
    "6a89243a-eb59-4b1c-8dff-951c2abdf88a/A3dgeWA21pYcaWU7zz04WHo7scvo0fe9nyojZjdp"
);

builder.Services.AddSingleton<IFeatureHubConfig>(featureHubConfig);
// Register IClientContext from FeatureHub as scoped
builder.Services.AddScoped<IClientContext>(provider =>
{
    var config = provider.GetRequiredService<IFeatureHubConfig>();
    var clientContextTask = config.NewContext()
        .UserKey("example-user-key")
        .Platform(StrategyAttributePlatformName.Windows)
        .Country(StrategyAttributeCountryName.Denmark)
        .Build();
    
    return clientContextTask.Result;
});

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
