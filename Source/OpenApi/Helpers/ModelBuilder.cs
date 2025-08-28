using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenApi.Helpers
{
    internal class ModelBuilder
    {
        public static string BuildFromUrl(string url)
        {
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                var response = client.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError($"Request failed. Status Code: {(int)response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                var content = response.Content.ReadAsStringAsync().Result;

                if (string.IsNullOrWhiteSpace(content))
                {
                    Logger.LogError("Response content is empty.");
                    return null;
                }

                return content;
            }
            catch (HttpRequestException ex)
            {
                Logger.LogError($"HTTP request error: {ex.Message}");
                Logger.LogError(ex.StackTrace);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                Logger.LogError($"Request timed out: {ex.Message}");
                Logger.LogError(ex.StackTrace);
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Unexpected error: {ex.Message}");
                Logger.LogError(ex.StackTrace);
                return null;
            }
        }

        public static string BuildFromFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    Logger.LogError("File path is null or empty.");
                    return null;
                }

                if (!File.Exists(filePath))
                {
                    Logger.LogError($"File not found: {filePath}");
                    return null;
                }

                var content = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(content))
                {
                    Logger.LogError($"File is empty: {filePath}");
                    return null;
                }

                return content;
            }
            catch (IOException ex)
            {
                Logger.LogError($"File read error: {ex.Message}");
                Logger.LogError(ex.StackTrace);
                return null;
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError($"Access denied to file: {filePath}");
                Logger.LogError(ex.StackTrace);
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Unexpected error: {ex.Message}");
                Logger.LogError(ex.StackTrace);
                return null;
            }
        }


    }

}
