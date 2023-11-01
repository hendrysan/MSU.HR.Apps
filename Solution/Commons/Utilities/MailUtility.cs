using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace Commons.Utilities
{
    public class MailUtility
    {
        public static async Task<bool> SendAsync()
        {
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                var config = builder.Build();

                var host = "mail.mitrasolutech.com";//config["Smtp:Host"];
                var port = "465";// config["Smtp:Port"];
                var username = "hendry.priyatno@mitrasolutech.com";//config["Smtp:Username"];
                var to = "hendry.priyatno@gmail.com";


                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient();
                mail.To.Add(to);
                mail.From = new MailAddress(username);
                mail.Subject = "test email";
                mail.IsBodyHtml = true;
                mail.Body = "test body";
                SmtpServer.Host = host;
                SmtpServer.Port = Convert.ToInt32(port);
                SmtpServer.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                SmtpServer.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                await Loggers.DiscordLogger.SendAsync("MailUtility", ex);
                return false;
            }
        }
    }
}
