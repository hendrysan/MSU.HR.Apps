using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace Commons.Utilities
{
    public class MailUtility
    {
        public static async Task<bool> SendAsync(List<string> recipients, string subject, string body)
        {
            try
            {

                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                string fromAddress = config["Smtp:FormAddress"] ?? ""; ;
                string displayName = config["Smtp:DisplayName"] ?? "";

                MailMessage email = new MailMessage();
                email.From = new MailAddress(fromAddress, displayName);

                foreach (var recipient in recipients)
                {
                    email.To.Add(recipient);
                }

                email.Subject = subject;
                email.Body = body;
                email.IsBodyHtml = true;

                var task = await SmtpClient(email, config);

                return task;

            }
            catch (Exception ex)
            {
                await Loggers.DiscordLogger.SendAsync("MailUtility", ex);
                return false;
            }
        }

        private static async Task<bool> SmtpClient(MailMessage email, IConfigurationRoot config)
        {
            string userName = config["Smtp:UserName"] ?? "";
            string password = config["Smtp:Password"] ?? "";
            string smtpHost = config["Smtp:Host"] ?? "";
            int smtpPort = Convert.ToInt32(config["Smtp:Port"]);

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
