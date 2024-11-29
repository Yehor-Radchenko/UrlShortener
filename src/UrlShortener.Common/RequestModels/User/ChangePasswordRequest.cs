using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Common.RequestModels.User;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "User ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive number")]
    public int UserId { get; set; }

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