using OpenApi.Helpers;
using OpenApi.SourceBuilders;
using System;

namespace OpenApi
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            args = new string[] { "angular", "https://bo-legal-server-test-legal-person.apps.lab.notary.ir/swagger/v1/swagger.json", SampleHelper.GetSampleOutput() };
            Logger.LogInfo("DEBUG Mode");
#endif

            if (args.Length < 3)
            {
                Logger.LogInfo("Usage: OpenApi <type> (<url> | <file>) <outputPath>");
                Logger.LogInfo("Example (URL):  OpenApi angular \"https://example.com/api.json\" \"C:\\output\"");
                Logger.LogInfo("Example (File): OpenApi angular \"C:\\api.json\" \"C:\\output\"");
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
            {
                Logger.LogError("[ERROR] Failed to parse OpenAPI specification. The input may be invalid or empty.");
                return;
            }

            // Debug output
            Logger.LogInfo("Type: " + type);
            Logger.LogInfo("Read As: " + secondArg);
            Logger.LogInfo("Output Path: " + outputPath);

            if (type.Trim().Equals("angular", StringComparison.OrdinalIgnoreCase))
            {
                AngularSourceBuilder.Build(rootobject, outputPath);
            }
        }

    }
}
