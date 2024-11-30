using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Common.RequestModels.Url;

public class CreateUrlShortenerRequest
{
    [Required(ErrorMessage = "Full URL is required")]
    [MinLength(2, ErrorMessage = "Full URL must be at least 2 characters long")]
    [Url(ErrorMessage = "Full URL must be a valid URL")]
    [DataType(DataType.Url)]
    public string FullUrl { get; set; }
}