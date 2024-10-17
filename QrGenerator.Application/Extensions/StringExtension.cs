using System.Text.RegularExpressions;

namespace QrGenerator.Application.Extensions;

internal static class StringExtension
{
    internal static string GetDomain(this string url)
    {
        string pattern = @"^(?:https?:\/\/)?(?:www\.)?([^\/]+)";

        var regex = new Regex(pattern);
        return regex.Match(url).Value;
    }
}