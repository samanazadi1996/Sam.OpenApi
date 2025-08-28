using OpenApi.Helpers;
using OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace OpenApi.SourceBuilders
{
    internal class AngularSourceBuilder
    {
        public static JsonElement root;
        public static AngularSettingsDto settings;
        public static void Build(string openApiDocument, string outputPath)
        {
            root = JsonDocument.Parse(openApiDocument).RootElement;
            settings = AngularSettingsDto.Build();

            GenerateServices(outputPath);
            GenerateInterfaces(outputPath);
        }

        private static void GenerateServices(string outputPath)
        {
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
                List<string> imports = new List<string>();

                foreach (var (method, path, operation) in tagsGroup[tag])
                {
                    var func = TemplateReader.Angular.Any(method);

                    var funcName = operation.TryGetProperty("operationId", out var opId)
                        ? opId.GetString()
                        : GenerateFunctionName(method, path);

                    GetParameters(operation, out string querySrting, out string parameters);

                    GetBody(operation, out string bodyType);

                    GetTResponse(operation, out string tResponse);

                    var bodyModel = "body";

                    if (!string.IsNullOrEmpty(bodyType))
                    {
                        imports.Add(
                            TemplateReader.Angular.Any("import")
                            .Replace("ClassName", bodyType)
                            .Replace("FileName", settings.InterfacesPath + "/" + bodyType.ToKebabCase())
                            );

                        var tmp = (parameters + "").Split(',')
                            .Where(p => !string.IsNullOrWhiteSpace(p))
                            .ToList();

                        tmp.Add($"body : {bodyType}");

                        parameters = string.Join(" ,", tmp);
                    }
                    else
                    {
                        bodyModel = "{ }";
                    }

                    if (!string.IsNullOrEmpty(tResponse))
                    {
                        imports.Add(
                            TemplateReader.Angular.Any("import")
                            .Replace("ClassName", tResponse)
                            .Replace("FileName", settings.InterfacesPath + "/" + tResponse.ToKebabCase())
                            );

                        tResponse = $"<{tResponse}>";
                    }

                    sb.Append(
                    func.Replace("FunctionName", funcName)
                        .Replace("EndpointUrl", path + querySrting)
                        .Replace("TRequest", parameters)
                        .Replace("<TResponse>", tResponse)
                        .Replace("BodyModel", bodyModel)
                        );

                    sb.AppendLine();
                }

                var result = template.Replace("ServiceName", tag.ToPascalCase())
                    .Replace("Functions", sb.ToString())
                    .Replace("Imports", string.Join(Environment.NewLine, imports.Distinct()));

                var filePath = Path.Combine(outputPath, $"{tag.ToKebabCase()}.service.ts");
                File.WriteAllText(filePath, result);
            }

            Logger.LogSuccess("TypeScript clients generated.");

            string GenerateFunctionName(string method, string path)
            {
                var cleanPath = path.Replace("/", "_").Replace("{", "").Replace("}", "");
                return $"{method.ToLower()}{cleanPath}".ConvertCase(settings.FunctionNameConvention);
            }
        }

        private static void GetTResponse(JsonElement operation, out string tResponse)
        {
            tResponse = null;
            try
            {
                if (operation.TryGetProperty("responses", out var responses))
                    if (responses.TryGetProperty("200", out var ok))
                        if (ok.TryGetProperty("content", out var content))
                            if (content.TryGetProperty("application/json", out var applicationJson))
                                if (applicationJson.TryGetProperty("schema", out var schema))
                                    if (schema.TryGetProperty("$ref", out var @ref))
                                        tResponse = RefToName(@ref.ToString());
            }
            catch
            {
            }
        }

        private static void GetParameters(JsonElement operation, out string queryString, out string parameters)
        {
            queryString = null;
            parameters = null;

            List<(string Name, string @Type)> props = new List<(string Name, string Type)>();
            try
            {
                if (operation.TryGetProperty("parameters", out var temp))
                {
                    foreach (var item in temp.EnumerateArray())
                        if (item.TryGetProperty("in", out var type))
                            if (type.ToString() == "query" && item.TryGetProperty("name", out var paramName))
                                if (item.TryGetProperty("schema", out var paramSchema))
                                    if (paramSchema.TryGetProperty("type", out var paramType))
                                        props.Add((paramName.ToString(), GeneratePrimitiveSample(paramType.ToString())));
                }
                if (props.Any())
                {
                    parameters = string.Join(",", props.Select(p => $"{p.Name.ToCamelCase()} :{p.Type}"));

                    queryString = "?" + string.Join("&", props.Select(p => $"{p.Name}=${{{p.Name.ToCamelCase()}}}"));
                }
            }
            catch
            {
            }
        }

        private static void GetBody(JsonElement operation, out string bodyType)
        {
            bodyType = null;
            try
            {
                if (operation.TryGetProperty("requestBody", out var requestBody))
                    if (requestBody.TryGetProperty("content", out var content))
                        if (content.TryGetProperty("application/json", out var applicationJson))
                            if (applicationJson.TryGetProperty("schema", out var schema))
                                if (schema.TryGetProperty("$ref", out var @ref))
                                    bodyType = RefToName(@ref.ToString());
            }
            catch
            {
            }

        }

        public static void GenerateInterfaces(string outputPath)
        {
            var targetDir = Path.Combine(outputPath, settings.InterfacesPath);
            Directory.CreateDirectory(targetDir);

            if (!root.TryGetProperty("components", out var components) ||
                !components.TryGetProperty("schemas", out var schemas))
            {
                Logger.LogError("No schemas found in OpenAPI JSON.");
                return;
            }
            List<string> Imports = new List<string>();

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

            string GenerateModel(JsonElement value)
            {
                if (value.TryGetProperty("$ref", out var tmp))
                {
                    var className = RefToName(tmp.ToString());

                    if (className.Contains("<")) // generic
                        return className;

                    Imports.Add(
                        TemplateReader.Angular.Any("import")
                        .Replace("ClassName", className)
                        .Replace("FileName", className.ToKebabCase())
                    );

                    return className;
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



            string CheckNullable(JsonElement value)
            {
                var nullable = false;

                if (value.TryGetProperty("nullable", out var tmp))
                    if (bool.TryParse(tmp.ToString().ToLower(), out bool bullableBool))
                        nullable = bullableBool;

                return nullable ? "?" : "";
            }

        }

        private static string RefToName(string v)
        {
            return CleanName(v).Split('/').LastOrDefault();
        }
        private static string GeneratePrimitiveSample(string type)
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

        private static string CleanName(string name)
        {
            // حذف generic ها مثل PagedResult`1[Product]
            int backtickIndex = name.IndexOf('`');
            if (backtickIndex > 0)
                name = name.Substring(0, backtickIndex);

            // اگر generic بود (مثلاً PagedResult[Product])
            if (name.Contains("[") && name.Contains("]"))
            {
                var baseName = name.Substring(0, name.IndexOf("["));
                var inner = name.Substring(name.IndexOf("[") + 1, name.LastIndexOf("]") - name.IndexOf("[") - 1);

                var innerTypes = inner.Split(',')
                    .Select(t => CleanName(t)) // recursive
                    .ToList();

                return $"{baseName}<{string.Join(", ", innerTypes)}>";
            }

            // حذف کاراکترهای اضافی
            name = name
                .Replace("[", "")
                .Replace("]", "")
                .Replace(",", "")
                .Replace(".", "-")
                .Replace(" ", "")
                .Replace("`", "")
                .Trim();

            if (name.Contains("-"))
            {
                var parts = name.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                name = parts[parts.Length - 1];
            }

            return name + "Interface";
        }

    }
}
