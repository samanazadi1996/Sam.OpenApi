using Sam.OpenApi.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sam.OpenApi.Helpers
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
                    Logger.LogError($"[ERROR] Request failed. Status Code: {(int)response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                var content = response.Content.ReadAsStringAsync().Result;

                if (string.IsNullOrWhiteSpace(content))
                {
                    Logger.LogError("[ERROR] Response content is empty.");
                    return null;
                }

                return content;
            }
            catch (HttpRequestException ex)
            {
                Logger.LogError($"[ERROR] HTTP request error: {ex.Message}");
                Logger.LogError(ex.StackTrace);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                Logger.LogError($"[ERROR] Request timed out: {ex.Message}");
                Logger.LogError(ex.StackTrace);
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogError($"[ERROR] Unexpected error: {ex.Message}");
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
                    Logger.LogError("[ERROR] File path is null or empty.");
                    return null;
                }

                if (!File.Exists(filePath))
                {
                    Logger.LogError($"[ERROR] File not found: {filePath}");
                    return null;
                }

                var content = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(content))
                {
                    Logger.LogError($"[ERROR] File is empty: {filePath}");
                    return null;
                }

                return content;
            }
            catch (IOException ex)
            {
                Logger.LogError($"[ERROR] File read error: {ex.Message}");
                Logger.LogError(ex.StackTrace);
                return null;
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError($"[ERROR] Access denied to file: {filePath}");
                Logger.LogError(ex.StackTrace);
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogError($"[ERROR] Unexpected error: {ex.Message}");
                Logger.LogError(ex.StackTrace);
                return null;
            }
        }


    }

}
