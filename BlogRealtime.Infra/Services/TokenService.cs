using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogRealtime.Domain.Entity;
using BlogRealtime.Domain.Services;
using BlogRealtime.Domain.Settings;
using Microsoft.IdentityModel.Tokens;

namespace BlogRealtime.Infra.Services;

public class TokenService : ITokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public TokenService(JwtSettings jwtSettings)
    {
        _secretKey = jwtSettings.SecretKey;
        _issuer = jwtSettings.Issuer;
        _audience = jwtSettings.Audience;
        _expirationMinutes = jwtSettings.ExpirationMinutes;
    }

    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
