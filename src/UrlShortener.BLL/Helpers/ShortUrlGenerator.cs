using System.Text;

namespace UrlShortener.BLL.Helpers;

public static class UrlHasher
{
    private static readonly Random random = new();
    private const string AllPossibleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string GenerateShortUrlPath(int lengthOfShortUrl = 6)
    {
        if (lengthOfShortUrl <= 0)
            throw new ArgumentException("Length of short url should be greater than zero.");

        var shortUrlBuilder = new StringBuilder();

        for (int i = 0; i < lengthOfShortUrl; i++)
        {
            shortUrlBuilder.Append(AllPossibleCharacters[random.Next(AllPossibleCharacters.Length)]);
        }

        return shortUrlBuilder.ToString();
    }
}
