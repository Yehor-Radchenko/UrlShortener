using UrlShortener.Common.RequestModels.Url;
using UrlShortener.Common.ResponseModels.Url;

namespace UrlShortener.BLL.Interfaces;

public interface IUrlService
{
    Task<UrlViewModel> GetByShortedUrlAsync(string shortedUrl);

    Task<IEnumerable<UrlViewModel>> GetAllAsync();

    Task<UrlViewModel> GetByIdAsync(int id);

    Task<int> AddAsync(CreateUrlShortenerRequest requestModel, int userId);

    Task UpdateAsync(UpdateUrlShortenerRequest requestModel, int userId);

    Task DeleteAsync(int id);
}
