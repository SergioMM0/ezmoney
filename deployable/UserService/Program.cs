using Messages.RPC;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using RPC;
using RPC.RpcFactory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// HTTP Circuit breakers

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<RpcTimeoutException>()
    .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromMicroseconds(Math.Pow(2, retryAttempt)));

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<RpcTimeoutException>()
    .CircuitBreakerAsync(1, TimeSpan.FromMicroseconds(1));

var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMicroseconds(1), (context, timeSpan, task) => {
    return Task.FromException<HttpResponseMessage>(new RpcTimeoutException("Request timed out."));
});

var policies = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);
builder.Services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(policies);


// Configure topics
builder.Services.AddSingleton<IConnectionFactoryProvider, RpcFactory>();
builder.Services.Configure<Topics>(builder.Configuration.GetSection("RPCMessages"));
builder.Services.AddSingleton<RpcClient>(provider =>
    new RpcClient(
        provider.GetRequiredService<IOptions<Topics>>().Value.Topic,
        provider.GetRequiredService<IConnectionFactoryProvider>()
    )
);

builder.Services.AddSingleton<Topics>(sp =>
    sp.GetRequiredService<IOptions<Topics>>().Value);
var app = builder.Build();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
