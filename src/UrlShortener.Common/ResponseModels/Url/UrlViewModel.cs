namespace UrlShortener.Common.ResponseModels.Url;

public class UrlViewModel
{
    public int Id { get; set; }

    public string FullUrl { get; set; }

    public string ShortUrl { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime LastAppeal { get; set; }

    public int NumberOfAppeals { get; set; }

    public int UserId { get; set; }
}
