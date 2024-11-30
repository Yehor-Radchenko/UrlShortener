using Microsoft.Extensions.Configuration;

namespace UrlShortener.BLL.Services;

public class UrlCastService(IConfiguration configuration)
{
    public string CastUrl(string shortUrlPath)
    {
        string domain = configuration["domain"]!;

        return $"{domain}{shortUrlPath}";
    }
}