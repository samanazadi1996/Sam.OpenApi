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

            str = str.Replace(" ", "-").Replace("_", "-");

            str = Regex.Replace(str, "([a-z0-9])([A-Z])", "$1-$2");

            str = Regex.Replace(str, "-{2,}", "-");

            return str.ToLower();
        }
        public static string ToSnakeCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            str = str.Replace(" ", "_").Replace("-", "_");

            str = Regex.Replace(str, "([a-z0-9])([A-Z])", "$1_$2");

            str = Regex.Replace(str, "_{2,}", "_");

            return str.ToLower();
        }

        public static string ConvertCase(this string str, string caseType)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            switch (caseType?.ToLowerInvariant())
            {
                case "pascal":
                    return str.ToPascalCase();
                case "kebab":
                    return str.ToKebabCase();
                case "snake":
                    return str.ToSnakeCase();
                case "camel":
                    return str.ToCamelCase();
                default:
                    return str;
            }
        }

    }

}