using System.Text.RegularExpressions;

namespace Serwis.Models.Extensions
{
    public static class StringExtension
    {
        public static string StripHTML(this string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}
