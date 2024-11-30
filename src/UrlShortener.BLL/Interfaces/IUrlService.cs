using UrlShortener.Common.RequestModels.Url;
using UrlShortener.Common.ResponseModels.Url;

namespace UrlShortener.BLL.Interfaces;

public interface IUrlService
{
    Task<UrlViewModel> GetByShortedUrlAsync(string shortedUrl, int userId);

    Task<IEnumerable<UrlViewModel>> GetAllAsync(int userId);

    Task<UrlViewModel> GetByIdAsync(int id, int userId);

    Task<string> AddAsync(CreateUrlShortenerRequest requestModel, int userId);

    Task UpdateAsync(UpdateUrlShortenerRequest requestModel, int userId);

    Task DeleteAsync(int id, int userId);
}
