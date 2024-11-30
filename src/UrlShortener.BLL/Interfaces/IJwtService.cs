namespace UrlShortener.BLL.Interfaces;

public interface IJwtService
{
    string GenerateToken(string userId, string userEmail, IEnumerable<string> roles);
}
