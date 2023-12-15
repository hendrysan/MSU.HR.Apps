using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Commons.Loggers
{
    public class DiscordLogger
    {
        private static readonly int maxLength = 2000;

        public DiscordLogger()
        {
        }

        public static async Task<System.Net.HttpStatusCode> SendAsync(string serviceName, string message, object? query = null)
        {
            string content = string.Empty;
            Stream? file = null;

            content += "**Service Name**";
            content += $"```{serviceName}```";

            content += "**Message**";
            content += $"```{message}```";


            if (query != null && content.Length <= maxLength)
            {
                content += $"**Query Name : {query.GetType().Name}**";
                file = GetStream(query);
            }

            return await PostContent(content, file);
        }

        public static async Task<System.Net.HttpStatusCode> SendAsync(string serviceName, Exception ex, HttpContext? http = null, object? query = null)
        {
            var exception = ExceptionInformation(ex);
            Stream? file = null;

            string content = string.Empty;

            content += "**Service Name**";
            content += $"```{serviceName}```";

            content += "**Exception**";
            content += $"```{exception}```";

            if (http != null)
            {
                content += "**Request**";
                content += $"```{http.Request.Host} " +
                    $"{http.Request.Path.HasValue}```";

            }

            if (query != null && content.Length <= maxLength)
            {
                content += $"**Query Name : {query.GetType().Name}**";
                file = GetStream(query);
            }

            return await PostContent(content, file);
        }

        private static Stream? GetStream(object query)
        {
            Stream? file = new MemoryStream();
            var writer = new StreamWriter(file);
            writer.Write(JsonSerializer.Serialize(query, _defaultJsonSerializerOptions));
            writer.Flush();
            file.Position = 0;
            return file;
        }


        private static async Task<System.Net.HttpStatusCode> PostContent(string content, Stream? file = null)
        {

            if (content.Length >= maxLength)
            {
                _ = content[..(maxLength - 1)];
            }

            HttpClient client = new();
            var form = new MultipartFormDataContent
            {
                { new StringContent(content), "content" }
            };


            if (file != null)
            {
                var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                byte[] byteArray = memoryStream.ToArray();
                var fileContent = new ByteArrayContent(byteArray);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                form.Add(fileContent, "Document", fileName: "body.txt");
            };


            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var config = builder.Build();

            string? baseUrl = config["Discord:Url"];// "";
            var guildId = config["Discord:GuildId"];
            string? tokenWebhook = config["Discord:Token"];

            var response = await client.PostAsync($"{baseUrl}/{guildId}/{tokenWebhook}", form);
            //var data = await response.Content.ReadAsStringAsync();

            return response.StatusCode;
        }

        private static readonly JsonSerializerOptions _defaultJsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private static string ExceptionInformation(Exception ex)
        {
            var obj = new
            {
                Type = ex.GetType().FullName,
                ex.Message,
                ex.Source,
                StackTrace = ex.StackTrace != null,
                InnerException = ex.InnerException != null ? ExceptionInformation(ex.InnerException) : null
            };

            return JsonSerializer.Serialize(obj, _defaultJsonSerializerOptions);
        }
    }
}
