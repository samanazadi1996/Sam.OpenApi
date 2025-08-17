using System.Linq;
using System.Text.RegularExpressions;

namespace OpenApi.Helpers
{
    public static class StringHelper
    {
        public static string ToPascalCase(this string str)
        {
            return string.Concat(str.Split(' ', '-', '_').Select(w => char.ToUpper(w[0]) + w.Substring(1)));
        }

        public static string ToCamelCase(this string str)
        {
            var pascal = ToPascalCase(str);
            return char.ToLower(pascal[0]) + pascal.Substring(1);
        }

        public static string ToKebabCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            // جایگزینی فاصله و _
            str = str.Replace(" ", "-").Replace("_", "-");

            // جدا کردن حروف PascalCase / camelCase
            str = Regex.Replace(str, "([a-z0-9])([A-Z])", "$1-$2");

            // چندتا - پشت سر هم → یکی
            str = Regex.Replace(str, "-{2,}", "-");

            return str.ToLower();
        }

    }

}
