using System.Net;
using Messages.Auth;
using Messages.User;
using Messages.User.Request;
using Messages.User.Response;
using Microsoft.AspNetCore.Authentication;
using OpenTelemetry.Trace;

namespace AuthService.Services;

public class AuthService {
    private readonly JWTTokenService _jwtTokenService;
    private readonly AuthServiceConfiguration _config;
    private readonly HttpClient _client;
    private readonly Tracer _tracer;
    
    public AuthService(JWTTokenService jwtTokenService, 
        AuthServiceConfiguration config, 
        HttpClient client,
        Tracer tracer) {
        _jwtTokenService = jwtTokenService;
        _config = config;
        _client = client;
        _tracer = tracer;
    }
    
    public async Task<string> Register(RegisterUserReq request) {
        using var activity = _tracer.StartActiveSpan("Register - Service");
        // 1. Parse the RegisterUserReq to a CreateUserReq
        var createUserReq = new CreateUserReq {
            Name = request.Name,
            PhoneNumber = request.PhoneNumber
        };
        
        //2. Send HTTP request to user service to register (create) a user
        var response = await _client.PostAsJsonAsync(_config.CreateUserUrl, createUserReq);
        if (!response.IsSuccessStatusCode) {
            throw new Exception("Error creating user. User service returned: " + response.StatusCode);
        }
        
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        Console.WriteLine("user created: " + user!.Id + " " + user.Name + " " + user.PhoneNumber);
        
        //3. Create a token for the user (with user data)
        var token = _jwtTokenService.CreateToken(user);
        return token.Value;
    }
    
    public async Task<AuthenticationToken> Login(LoginUserReq request) {
        using var activity = _tracer.StartActiveSpan("Login - Service");
        //1. Send HTTP request to user service to login a user
        var response = await _client.GetAsync(_config.GetUserByPhoneNumberUrl + request.PhoneNumber);
        if (!response.IsSuccessStatusCode) {
            throw new Exception("Error logging in. User service returned: " + response.StatusCode);
        }
        
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        Console.WriteLine("user logged in: " + user!.Id + " " + user.Name + " " + user.PhoneNumber);
        
        //2. Create a token for the user (with user data)
        var token = _jwtTokenService.CreateToken(user);
        return token;
    }
}
