using Sam.OpenApi.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Sam.OpenApi.Angular
{
    internal class AngularSourceBuilder
    {
        public static void Generate(string openApiDocument, string outputPath)
        {

            var doc = JsonDocument.Parse(openApiDocument);
            var root = doc.RootElement;

            var paths = root.GetProperty("paths");
            var tagsGroup = new Dictionary<string, List<(string method, string path, JsonElement operation)>>();

            foreach (var pathProp in paths.EnumerateObject())
            {
                string path = pathProp.Name;
                var methods = pathProp.Value;

                foreach (var methodProp in methods.EnumerateObject())
                {
                    string method = methodProp.Name;
                    var operation = methodProp.Value;

                    if (!operation.TryGetProperty("tags", out var tagsArray)) continue;

                    foreach (var tagEl in tagsArray.EnumerateArray())
                    {
                        string tag = tagEl.GetString();
                        if (!tagsGroup.ContainsKey(tag))
                            tagsGroup[tag] = new List<(string, string, JsonElement)>();

                        tagsGroup[tag].Add((method.ToUpper(), path, operation));
                    }
                }
            }

            Directory.CreateDirectory(outputPath);
            var template = TemplateReader.Angular.Service;

            foreach (var tag in tagsGroup.Keys)
            {

                var sb = new StringBuilder();

                foreach (var (method, path, operation) in tagsGroup[tag])
                {
                    var func = TemplateReader.Angular.Any(method);

                    var funcName = operation.TryGetProperty("operationId", out var opId)
                        ? opId.GetString()
                        : GenerateFunctionName(method, path);

                    sb.Append(
                    func.Replace("FunctionName", funcName)
                        .Replace("EndpointUrl", path));

                    sb.AppendLine();
                }

                var result = template.Replace("ServiceName", tag.ToPascalCase())
                    .Replace("Functions", sb.ToString());

                var filePath = Path.Combine(outputPath, $"{tag.ToKebabCase()}.service.ts");
                File.WriteAllText(filePath, result);
            }

            Logger.LogSuccess("TypeScript clients generated.");

        }

        static string GenerateFunctionName(string method, string path)
        {
            var cleanPath = path.Replace("/", "_").Replace("{", "").Replace("}", "");
            return $"{method.ToLower()}{cleanPath}";
        }

    }
}
