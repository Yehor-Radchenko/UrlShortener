using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Common.RequestModels.Url;

public class UpdateUrlShortenerRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "FullUrl is required")]
    [MinLength(2, ErrorMessage = "FullUrl must be at least 2 characters long")]
    public string FullUrl { get; set; }

    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Short URL can only contain letters and numbers")]
    [StringLength(30, MinimumLength = 1, ErrorMessage = "Short URL must be between 1 and 30 characters")]
    public string? ShortUrl { get; set; }
}