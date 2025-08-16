using Sam.OpenApi.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sam.OpenApi.Helpers
{
    internal class ModelBuilde
    {
        public static string BuildFromUrl(string url)
        {
            try
            {
                var client = new HttpClient();
                var response = client.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[ERROR] Request failed. Status Code: {(int)response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                var content = response.Content.ReadAsStringAsync().Result;

                if (string.IsNullOrWhiteSpace(content))
                {
                    Console.WriteLine("[ERROR] Response content is empty.");
                    return null;
                }

                return content;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[ERROR] HTTP request error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"[ERROR] Request timed out: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Unexpected error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        public static string BuildFromFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    Console.WriteLine("[ERROR] File path is null or empty.");
                    return null;
                }

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"[ERROR] File not found: {filePath}");
                    return null;
                }

                var content = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(content))
                {
                    Console.WriteLine($"[ERROR] File is empty: {filePath}");
                    return null;
                }

                return content;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[ERROR] File read error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"[ERROR] Access denied to file: {filePath}");
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Unexpected error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }


    }

}
