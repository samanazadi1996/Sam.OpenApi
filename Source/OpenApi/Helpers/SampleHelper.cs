using System;
using System.IO;
using System.Linq;

namespace OpenApi.Helpers
{
    internal static class SampleHelper
    {
        public static string GetSampleDirectory()
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string dir = currentDir;

            while (dir != null && !Directory.GetFiles(dir, "*.sln").Any())
            {
                dir = Directory.GetParent(dir)?.FullName;
            }

            return Path.Combine(dir, "OpenApiSample");
        }
        public static string GetSampleOpenApiJson()
        {
            string dir = GetSampleDirectory();
            return Path.Combine(dir, "open-api.json");

        }

        internal static string GetSampleOutput()
        {
            string dir = GetSampleDirectory();
            return Path.Combine(dir, "output");
        }
    }
}
