using System.Globalization;
using System.Text.RegularExpressions;

namespace PgEntityGenerator.Helpers
{
    public static class StringExtensions
    {
        public static string ToPascalCase(this string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return default;
            }

            var formatted = input.ToLower().Replace("_", " ");
            var info = CultureInfo.InvariantCulture.TextInfo;
            var result = info.ToTitleCase(formatted).Replace(" ", string.Empty);
            return result;
        }

        public static bool TryParseSequence(this string defaultValue, out string sequence)
        {
            const string pattern = @"^nextval\('(.*)'::regclass\)$";
            var match = Regex.Match(defaultValue, pattern);
            if (match.Success && match.Length > 1) 
            {
                sequence = match.Groups[1].Value;
                return true;
            }

            sequence = null;
            return false;
        }
    }
}