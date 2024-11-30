using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UrlShortener.BLL.Interfaces;

namespace UrlShortener.BLL.Services;

public class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly string _secretKey = configuration["JwtSettings:SecretKey"]!;
    private readonly string _issuer = configuration["JwtSettings:Issuer"]!;
    private readonly string _audience = configuration["JwtSettings:Audience"]!;
    private readonly string _expiresInHours = configuration["JwtSettings:ExpiresInHours"]!;

    public string GenerateToken(string userId, string userEmail, IEnumerable<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, userEmail),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(int.Parse(_expiresInHours)),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}