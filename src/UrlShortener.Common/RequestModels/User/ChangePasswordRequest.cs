using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Common.RequestModels.User;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Old password is required")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "New password is required")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "New password must be at least 8 characters long")]
    [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).{8,}$",
        ErrorMessage = "New password must contain at least one letter and one digit")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm new password is required")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match")]
    public string ConfirmNewPassword { get; set; }
}