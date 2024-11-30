using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.DAL.Entities;

public class Url : IEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string FullUrl { get; set; } = string.Empty;

    [Required]
    public string ShortUrl { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedDate { get; set; }

    public DateTime? LastAppeal { get; set; }

    [DefaultValue(0)]
    public int NumberOfAppeals { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public User? User { get; set; }
}
