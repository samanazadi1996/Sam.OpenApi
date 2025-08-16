using System.IO;

namespace Sam.OpenApi.Helpers
{
    public static class TemplateReader
    {
        public static class Angular
        {
            public static string Service => ReadFile(nameof(Angular), nameof(Service));
            public static string Get => ReadFile(nameof(Angular), nameof(Get));
            public static string Post => ReadFile(nameof(Angular), nameof(Post));
            public static string Put => ReadFile(nameof(Angular), nameof(Put));
            public static string Delete => ReadFile(nameof(Angular), nameof(Delete));
            public static string Any(string name) => ReadFile(nameof(Angular), name.ToLower());
        }


        static string ReadFile(string type, string name)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", type, name + ".txt");
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            throw new System.Exception("Template NotFound -> " + path);
        }
    }
}
