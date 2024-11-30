using Microsoft.AspNetCore.Mvc;
using UrlShortener.BLL.Interfaces;
using UrlShortener.Common.RequestModels.User;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController(
    IAccountService accountService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        var token = await accountService.TryLoginUser(model);
        return Ok(new { token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUserRequest model)
    {
        var userId = await accountService.RegisterUser(model);
        return Ok(new { userId = userId });
    }
}