﻿using Messages.RPC;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using RPC;
using RPC.RpcFactory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// HERE we add the IHttpClientFactory
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("UserRepoHTTP", client => {
    client.BaseAddress = new Uri("http://user-repo:8080");
});
// HTTP Circuit breakers

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<RpcTimeoutException>()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryAttempt, context) => {
            Monitoring.Monitoring.Log.Information($"Retrying... attempt {retryAttempt}");
        });

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<RpcTimeoutException>()
    .CircuitBreakerAsync(1, TimeSpan.FromSeconds(30), onBreak: (outcome, timespan) =>
    {
        Monitoring.Monitoring.Log.Warning("UserService::Circuit breaker opened!");
    }, onReset: () =>
    {
        Monitoring.Monitoring.Log.Information("UserService::Circuit breaker reset!");
    });

var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30), (context, timeSpan, task) => {
    Monitoring.Monitoring.Log.Error("TimeoutPolicy::request timed out.");
    return Task.FromException<HttpResponseMessage>(new RpcTimeoutException("Request timed out."));
});

// Wrapping the policies in the correct order: timeoutPolicy should be the outermost
var policies = Policy.WrapAsync( circuitBreakerPolicy, retryPolicy, timeoutPolicy);
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

