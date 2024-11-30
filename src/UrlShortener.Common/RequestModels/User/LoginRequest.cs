using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Common.RequestModels.User;

public class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}