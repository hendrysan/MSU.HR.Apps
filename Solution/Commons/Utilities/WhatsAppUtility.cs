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

        public static async Task<System.Net.HttpStatusCode> SendAsync(string target, string message, string countryCode = "62")
        {
            var model = new WhatsAppModel
            {
                Target = target,
                CountryCode = countryCode,
                Message = message
            };

            try
            {
                var json = JsonSerializer.Serialize(model);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                string? baseUrl = config["WhatsApp:Url"];
                string? token = config["WhatsApp:Token"];

                HttpClient client = new();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", token);

                var response = await client.PostAsync($"{baseUrl}/send", data);
                var responseRead = await response.Content.ReadAsStringAsync();

                return response.StatusCode;
            }
            catch (Exception ex)
            {
                await Loggers.DiscordLogger.SendAsync("WhatsAppUtility SendAsync", ex, null, model);
                return System.Net.HttpStatusCode.BadRequest;
            }
        }
    }

    class WhatsAppModel
    {
        public string? Target { get; set; }
        public string? CountryCode { get; set; }
        public string? Message { get; set; }
    }


}
