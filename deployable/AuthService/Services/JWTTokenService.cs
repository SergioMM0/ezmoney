using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Messages.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public class JWTTokenService {
    private readonly AuthServiceConfiguration _config;
    
    public JWTTokenService(AuthServiceConfiguration config) {
        _config = config;
    }

    public AuthenticationToken CreateToken(UserResponse user) {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim> {
            new ("Id", user.Id.ToString()),
            new ("Name", user.Name),
            new ("PhoneNumber", user.PhoneNumber)
        };

        var tokenOptions = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        var authToken = new AuthenticationToken {
            Value = tokenString
        };
        return authToken;
    }
}

