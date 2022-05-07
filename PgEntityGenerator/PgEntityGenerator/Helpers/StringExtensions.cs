using System.Globalization;

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
    }
}
