using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.BLL.Interfaces;
using UrlShortener.Common.RequestModels.Url;
using UrlShortener.DAL.Entities;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UrlController(
    IUrlService urlService,
    UserManager<User> userManager) : ControllerBase
{
    [HttpGet("getUrls")]
    [Authorize]
    public async Task<IActionResult> GetAllUrls()
    {
        var urls = await urlService.GetAllAsync(int.Parse(userManager.GetUserId(User)!));
        return Ok(urls);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUrlById([FromRoute] int id)
    {
        return Ok(await urlService.GetByIdAsync(id, int.Parse(userManager.GetUserId(User)!)));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateUrl(CreateUrlShortenerRequest url)
    {
        var shortUrl = await urlService.AddAsync(url, int.Parse(userManager.GetUserId(User)!));
        return Ok(new { shortUrl = shortUrl });
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUrl(UpdateUrlShortenerRequest url)
    {
        await urlService.UpdateAsync(url, int.Parse(userManager.GetUserId(User)!));
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUrl([FromRoute] int id)
    {
        await urlService.DeleteAsync(id, int.Parse(userManager.GetUserId(User)!));
        return NoContent();
    }

    [HttpGet("nav/{shortedUrl}")]
    public async Task<RedirectResult> GetUrl([FromRoute] string shortedUrl)
    {
        var fullUrl = await urlService.GetByShortedUrlAsync(shortedUrl, int.Parse(userManager.GetUserId(User)!));

        return new RedirectResult(fullUrl.FullUrl);
    }
}