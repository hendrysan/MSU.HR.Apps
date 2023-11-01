using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Commons.Utilities
{
    public class WhatsAppUtility
    {

        public static async Task<System.Net.HttpStatusCode> SendAsync(string target, string countryCode, string message)
        {
            var model = new WhatsAppModel
            {
                target = target,
                countryCode = countryCode,
                message = message
            };

            try
            {
                var json = JsonSerializer.Serialize(model);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                var config = builder.Build();

                string? baseUrl = config["WhatsApp:Url"];
                string? token = config["WhatsApp:Token"];

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", token);

                var response = await client.PostAsync($"{baseUrl}/send", data);
                var responseRead = await response.Content.ReadAsStringAsync();

                return response.StatusCode;
            }
            catch (Exception ex)
            {
                await Loggers.DiscordLogger.SendAsync("WhatsAppUtility", ex, null, model);
                return System.Net.HttpStatusCode.BadRequest;
            }
        }
    }

    class WhatsAppModel
    {
        public string target { get; set; }
        public string countryCode { get; set; }
        public string message { get; set; }
    }


}
