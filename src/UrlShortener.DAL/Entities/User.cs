using Microsoft.AspNetCore.Identity;

namespace UrlShortener.DAL.Entities;

public class User : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public ICollection<Url>? URLs { get; set; } = new List<Url>();
}