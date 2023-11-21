using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace Commons.Utilities
{
    public class MailUtility
    {
        private static string GetConfig(string key)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var config = builder.Build();

            return config[key].ToString();
        }

        public static async Task<bool> SendAsync(List<string> recipients, string subject, string body)
        {
            try
            {
                string fromAddress = GetConfig("Smtp:FormAddress");
                string displayName = GetConfig("Smtp:DisplayName");

                MailMessage email = new MailMessage();
                email.From = new MailAddress(fromAddress, displayName);

                foreach (var recipient in recipients)
                {
                    email.To.Add(recipient);
                }

                email.Subject = subject;
                email.Body = body;
                email.IsBodyHtml = true;

                var task = await SmtpClient(email);

                return task;

            }
            catch (Exception ex)
            {
                await Loggers.DiscordLogger.SendAsync("MailUtility", ex);
                return false;
            }
        }

        private static async Task<bool> SmtpClient(MailMessage email)
        {
            string userName = GetConfig("Smtp:UserName");
            string password = GetConfig("Smtp:Password");
            string smtpHost = GetConfig("Smtp:Host");
            int smtpPort = Convert.ToInt32(GetConfig("Smtp:Port"));

            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.Credentials = new NetworkCredential(userName, password);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;


            await Task.Run(() =>
            {
                 smtpClient.Send(email);
            });

            return true;
        }

    }
}
