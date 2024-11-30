using UrlShortener.Common.RequestModels.User;

namespace UrlShortener.BLL.Interfaces;

public interface IAccountService
{
    Task<int> RegisterUser(CreateUserRequest model);

    Task<string> TryLoginUser(LoginRequest model);

    Task<bool> ChangePassword(ChangePasswordRequest model);
}
