using Messages.RPC;
using Microsoft.Extensions.Options;
using Monitoring;
using OpenTelemetry.Trace;
using Polly;
using Polly.Extensions.Http;
using RPC;
using RPC.RpcFactory;

var builder = WebApplication.CreateBuilder(args);
/* Tracer config **/
var serviceName = "ExpenseService";
var serviceVersion = "1.0.0";


builder.Services.AddOpenTelemetry().Setup(serviceName, serviceVersion);
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));
/* End tracer config */

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// HERE we add the IHttpClientFactory
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("ExpenseRepoHTTP", client => {
    client.BaseAddress = new Uri("http://expense-repo:8080");
});
// HTTP Circuit breakers

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<RpcTimeoutException>()
    .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), onRetry: (outcome, timespan, retryAttempt, context) =>
    {
        Console.WriteLine($"Retrying... attempt {retryAttempt}");
    });

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<RpcTimeoutException>()
    .CircuitBreakerAsync(1, TimeSpan.FromSeconds(30), onBreak: (outcome, timespan) =>
    {
        Console.WriteLine("Circuit breaker opened!");
    }, onReset: () =>
    {
        Console.WriteLine("Circuit breaker reset!");
    });

var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30), (context, timeSpan, task) => {
    return Task.FromException<HttpResponseMessage>(new RpcTimeoutException("Request timed out."));
});

// Wrapping the policies in the correct order: timeoutPolicy should be the outermost
var policies = Policy.WrapAsync( retryPolicy, circuitBreakerPolicy,timeoutPolicy);
builder.Services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(policies);

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
