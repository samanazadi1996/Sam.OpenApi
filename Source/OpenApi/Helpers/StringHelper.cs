using System.Linq;

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
            return string.Join("-", str.Split(' ', '_')).ToLower();
        }

    }

}
