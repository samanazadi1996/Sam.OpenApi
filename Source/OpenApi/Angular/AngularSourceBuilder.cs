using OpenApi.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace OpenApi.Angular
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
        public static void GenerateInterfaces(string openApiJson, string outputPath)
        {
            var targetDir = Path.Combine(outputPath, "interfaces");
            Directory.CreateDirectory(targetDir);

            var doc = JsonDocument.Parse(openApiJson);
            var root = doc.RootElement;

            if (!root.TryGetProperty("components", out var components) ||
                !components.TryGetProperty("schemas", out var schemas))
            {
                Logger.LogError("No schemas found in OpenAPI JSON.");
                return;
            }

            foreach (var schema in schemas.EnumerateObject())
            {
                string interfaceName = CleanName(schema.Name);

                var lines = new List<string>();


                if (schema.Value.TryGetProperty("properties", out var props))
                {
                    foreach (var prop in props.EnumerateObject())
                    {

                        lines.Add($"  {prop.Name + CheckNullable(prop.Value)}: {GenerateModel(prop.Value)};");
                    }
                }

                var content = TemplateReader.Angular.Any("interface")
                    .Replace("InterfaceName", interfaceName)
                    .Replace("Properties", string.Join(Environment.NewLine, lines))
                    .Replace("Imports", string.Join(Environment.NewLine, Imports))
                                        ;
                Imports.Clear();

                string fileName = interfaceName.ToKebabCase() + ".ts";
                string filePath = Path.Combine(targetDir, fileName);

                File.WriteAllText(filePath, content);
                Logger.LogSuccess($"Interface Generated: {filePath}");
            }
        }
        static string GenerateModel(JsonElement value)
        {
            if (value.TryGetProperty("$ref", out var tmp))
            {
                var www = RefToNameName(tmp.ToString());
                Imports.Add("import {" + www + "} from './" + www.ToKebabCase() + "';");

                return www;
            }
            if (value.TryGetProperty("type", out var tmp2) && tmp2.ToString() == "object")
            {
                return "any";
            }
            if (value.TryGetProperty("type", out var tmp3) && tmp3.ToString() == "array")
            {
                if (value.TryGetProperty("items", out var tmp5))
                    return GenerateModel(tmp5) + "[]";

                return "any[]";
            }
            if (value.TryGetProperty("type", out var tmp4))
            {
                return GeneratePrimitiveSample(tmp4.ToString());
            }

            return "any";


        }

        private static string RefToNameName(string v)
        {
            return CleanName(v).Split('/').LastOrDefault();
        }

        static string GeneratePrimitiveSample(string type)
        {
            switch (type)
            {
                case "string": return "string";
                case "integer":
                case "number": return "number";
                case "boolean": return "boolean";

                default: return "any";
            }
        }

        static string CheckNullable(JsonElement value)
        {
            var nullable = false;

            if (value.TryGetProperty("nullable", out var tmp))
                if (bool.TryParse(tmp.ToString().ToLower(), out bool bullableBool))
                    nullable = bullableBool;

            return nullable ? "?" : "";
        }

        public static List<string> Imports { get; set; } = new List<string>();

        private static string CleanName(string name)
        {
            // حذف generic ها مثل `1[...]`
            int backtickIndex = name.IndexOf('`');
            if (backtickIndex > 0)
                name = name.Substring(0, backtickIndex);

            // حذف کاراکترهای اضافی
            name = name
                .Replace("[", "")
                .Replace("]", "")
                .Replace(",", "")
                .Replace(".", "-")
                .Replace(" ", "")
                .Replace("`", "")
                .Trim();

            // فقط آخرین بخش namespace
            if (name.Contains("-"))
            {
                var parts = name.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                name = parts[parts.Length - 1];
            }

            return name + "Interface";
        }

    }
}
