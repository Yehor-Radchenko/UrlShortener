using System.ComponentModel.DataAnnotations;
using UrlShortener.Common.Validation;

namespace UrlShortener.Common.RequestModels.Url;

public class CreateUrlShortenerRequest
{
    [Required(ErrorMessage = "FullUrl is required")]
    [MinLength(2, ErrorMessage = "FullUrl must be at least 2 characters long")]
    public string FullUrl { get; set; }
}