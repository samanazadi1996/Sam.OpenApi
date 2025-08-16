using Sam.OpenApi.Angular;
using Sam.OpenApi.Helpers;
using Sam.OpenApi.Models;
using System;

namespace Sam.OpenApi
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            // for test
            args = new string[] { "angular", "https://clean-architecture.koyeb.app/swagger/v1/swagger.json", "C:\\output" };

            if (args.Length < 3)
            {
                Logger.LogInfo("Usage: Sam.OpenApi <type> (<url> | <file>) <outputPath>");
                Logger.LogInfo("Example (URL):  Sam.OpenApi angular \"https://example.com/api.json\" \"C:\\output\"");
                Logger.LogInfo("Example (File): Sam.OpenApi angular \"C:\\api.json\" \"C:\\output\"");
                return;
            }

            var type = args[0];
            var secondArg = args[1];
            var outputPath = args[2];

            string rootobject;

            if (Uri.IsWellFormedUriString(secondArg, UriKind.Absolute))
            {
                rootobject = ModelBuilder.BuildFromUrl(secondArg);
            }
            else if (System.IO.File.Exists(secondArg))
            {
                rootobject = ModelBuilder.BuildFromFile(secondArg);
            }
            else
            {
                Logger.LogError("[ERROR] The second argument must be either a valid URL or an existing file path.");
                return;
            }
            if (rootobject is null)
                return;

            // Debug output
            Logger.LogInfo("Type: " + type);
            Logger.LogInfo("Read As: " + secondArg);
            Logger.LogInfo("Output Path: " + outputPath);

            if (type.Trim().Equals("angular", StringComparison.OrdinalIgnoreCase))
            {
                AngularSourceBuilder.Generate(rootobject, outputPath);
            }
        }
    }
}
