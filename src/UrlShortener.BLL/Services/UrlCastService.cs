using Microsoft.Extensions.Configuration;

namespace UrlShortener.BLL.Services;

public class UrlCastService(IConfiguration configuration)
{
    public string CastUrl(string shortUrlPath)
    {
        string domain = configuration["HostingSettings:Domain"]!;

        return $"https://{domain}/Url/nav/{shortUrlPath}";
    }
}