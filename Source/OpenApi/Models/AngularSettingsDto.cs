using System.IO;
using System.Text.Json;

namespace OpenApi.Models
{
    public class AngularSettingsDto
    {
        public string InterfacesPath { get; set; }
        public string FunctionNameConvention { get; set; }

        public static AngularSettingsDto Build()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Angular", "angular-settings.json");
            if (File.Exists(path))
            {
                return JsonSerializer.Deserialize<AngularSettingsDto>(File.ReadAllText(path),
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }

            throw new System.Exception("angular-settings.json NotFound -> " + path);
        }
    }

}
