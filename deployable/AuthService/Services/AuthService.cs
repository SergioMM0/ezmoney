using System.Net;
using Messages.User;
using Microsoft.AspNetCore.Authentication;

namespace AuthService.Services;

public class AuthService {
    private readonly JWTTokenService _jwtTokenService;
    private readonly AuthServiceConfiguration _config;
    
    public AuthService(JWTTokenService jwtTokenService, AuthServiceConfiguration config) {
        _jwtTokenService = jwtTokenService;
        _config = config;
    }
    
    public async Task<AuthenticationToken> Register(RegisterUserReq request) {
        //1. Send HTTP request to user service to register (create) a user
        var client = new HttpClient();
        var response = await client.PostAsJsonAsync("http://user-service:8080/user/register", request);
        if (!response.IsSuccessStatusCode) {
            throw new Exception("Error creating user. User service returned: " + response.StatusCode);
        }
        
        UserResponse user = await response.Content.ReadFromJsonAsync<UserResponse>();
        Console.WriteLine("user created: " + user.Id + " " + user.Name + " " + user.PhoneNumber);
        
        //2. Create a token for the user (with user data)
        var token = _jwtTokenService.CreateToken(user);
        return token;
    }
    
    public async Task<AuthenticationToken> Login(LoginUserReq request) {
        //1. Send HTTP request to user service to login a user
        var client = new HttpClient();
        var response = await client.PostAsJsonAsync("http://user-service:8080/user/login", request);
        if (!response.IsSuccessStatusCode) {
            throw new Exception("Error logging in. User service returned: " + response.StatusCode);
        }
        
        UserResponse user = await response.Content.ReadFromJsonAsync<UserResponse>();
        Console.WriteLine("user logged in: " + user.Id + " " + user.Name + " " + user.PhoneNumber);
        
        //2. Create a token for the user (with user data)
        var token = _jwtTokenService.CreateToken(user);
        return token;
    }
}
