using UrlShortener.Common.RequestModels.Url;
using UrlShortener.Common.RequestModels.User;
using UrlShortener.Common.ResponseModels.Url;
using UrlShortener.Common.ResponseModels.User;
using UrlShortener.DAL.Entities;

namespace UrlShortener.BLL.Profile;

public class MappingProfile : AutoMapper.Profile
{
    public MappingProfile()
    {
        // Url
        CreateMap<CreateUrlShortenerRequest, Url>();
        CreateMap<UpdateUrlShortenerRequest, Url>();
        CreateMap<Url, UrlViewModel>();

        // User
        CreateMap<CreateUserRequest, User>();
        CreateMap<UpdateUserDataRequest, User>();
        CreateMap<User, UserViewModel>();
    }
}
