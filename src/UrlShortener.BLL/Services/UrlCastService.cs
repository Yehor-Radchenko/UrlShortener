using Microsoft.Extensions.Configuration;
using UrlShortener.BLL.Interfaces;

namespace UrlShortener.BLL.Services;

public class UrlCastService(IConfiguration configuration) : IUrlCastService
{
    public string CastUrl(string shortUrlPath)
    {
        string domain = configuration["HostingSettings:Domain"]!;

        return $"https://{domain}/Url/nav/{shortUrlPath}";
    }
}