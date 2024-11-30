using AutoMapper;
using UrlShortener.BLL.Helpers;
using UrlShortener.BLL.Interfaces;
using UrlShortener.Common.Exceptions;
using UrlShortener.Common.RequestModels.Url;
using UrlShortener.Common.ResponseModels.Url;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;
using UrlShortener.DAL.UoW;

namespace UrlShortener.BLL.Services;

public class UrlService(
    IMapper mapper,
    IGenericRepository<Url> urlRepo,
    IUnitOfWork unitOfWork,
    IUrlCastService urlCastService) : IUrlService
{
    public async Task<string> AddAsync(CreateUrlShortenerRequest requestModel, int userId)
    {
        ArgumentNullException.ThrowIfNull(requestModel);

        var url = mapper.Map<Url>(requestModel);

        url.ShortUrl = await GetUniqueShortUrlPath(urlRepo, userId);
        url.CreatedDate = DateTime.Now;
        url.UserId = userId;

        urlRepo.Create(url);
        await unitOfWork.SaveChangesAsync();

        return urlCastService.CastUrl(url.ShortUrl);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var url = await urlRepo.GetAsync(url => url.Id == id && url.UserId == userId);
        urlRepo.Delete(url);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<UrlViewModel>> GetAllAsync(int userId)
    {
        var urls = await urlRepo.GetAllAsNoTrackingAsync(url => url.UserId == userId);
        var viewModels = mapper.Map<IEnumerable<UrlViewModel>>(urls);

        foreach (var vm in viewModels)
        {
            vm.ShortUrl = urlCastService.CastUrl(vm.ShortUrl);
        }

        return viewModels;
    }

    public async Task<UrlViewModel> GetByIdAsync(int id, int userId)
    {
        var url = await urlRepo.GetAsNoTrackingAsync(url => url.Id == id && url.UserId == userId);
        var viewModel = mapper.Map<UrlViewModel>(url);

        if (viewModel != null)
        {
            viewModel.ShortUrl = urlCastService.CastUrl(viewModel.ShortUrl);
        }

        return viewModel;
    }

    public async Task<UrlViewModel> GetByShortedUrlAsync(string shortedUrl, int userId)
    {
        return mapper.Map<UrlViewModel>(
            await urlRepo.GetAsNoTrackingAsync(url =>
            url.ShortUrl == shortedUrl));
    }

    public async Task UpdateAsync(UpdateUrlShortenerRequest requestModel, int userId)
    {
        var url = await urlRepo.GetAsync(url => url.Id == requestModel.Id);

        if (await urlRepo.ExistsAsync(url => url.ShortUrl == requestModel.ShortUrl && url.UserId == userId)
            && !string.IsNullOrEmpty(requestModel.ShortUrl))
        {
            throw new ConflictException($"Url with shortener {requestModel.ShortUrl} already exists in shorteners list.");
        }

        mapper.Map(requestModel, url);

        urlRepo.Update(url);

        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateUrlAppealData(int urlId)
    {
        var url = await urlRepo.GetAsync(url => url.Id == urlId);
        url.LastAppeal = DateTime.Now;
        url.NumberOfAppeals++;

        await unitOfWork.SaveChangesAsync();
    }

    private static async Task<string> GetUniqueShortUrlPath(IGenericRepository<Url> urlRepo, int userId)
    {
        var generetedUrlPath = "";
        do
        {
            generetedUrlPath = UrlHasher.GenerateShortUrlPath();
        } while (await urlRepo.ExistsAsync(url => url.ShortUrl == generetedUrlPath && url.UserId == userId));

        return generetedUrlPath;
    }
}