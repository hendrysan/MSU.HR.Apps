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


                string pengirimEmail = "noreply.apps.001A@gmail.com";
                string pengirimPassword = "tkit kggl odxx ynue";
                //string pengirimPassword = "lezy yrmo yucm hfby";

                // Informasi penerima
                string penerimaEmail = "hendry.priyatno@mitrasolutech.com";

                // Konfigurasi SMTP Server
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;


                // Membuat objek MailMessage
                MailMessage email = new MailMessage();
                email.From = new MailAddress(pengirimEmail);
                email.To.Add(penerimaEmail);
                email.Subject = "Contoh Email";
                email.Body = "Ini adalah contoh email yang dikirim menggunakan C#.";

                // Mengirim email menggunakan SmtpClient
                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
                smtpClient.Credentials = new NetworkCredential(pengirimEmail, pengirimPassword);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Send(email);

                Console.WriteLine("Email berhasil dikirim.");
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
