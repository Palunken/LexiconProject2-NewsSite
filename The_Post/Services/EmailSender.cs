using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit.Text;
using System.Text.RegularExpressions;

namespace The_Post.Services
{
    
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
             try
             {
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:Port"]);
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);

                // Create email message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress(email, email));
                message.Subject = subject;

                var plainTextBody = Regex.Replace(htmlMessage, "<.*?>", string.Empty);

                // Create a multipart email with both HTML and plain text versions
                var multipart = new MultipartAlternative
                {
                    new TextPart(TextFormat.Plain) { Text = plainTextBody }, // Plain-text fallback
                    new TextPart(TextFormat.Html) { Text = htmlMessage } // HTML version
                };

                // Set email body
                message.Body = multipart;  

                using var smtpClient = new SmtpClient();
                await smtpClient.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(username, password);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
             }
            catch (Exception ex)
             {
                 Console.WriteLine($"Error sending email: {ex.Message}");
                 // Log the error (optional)
             }



        }
    }
}



//Email Adress:
            // The.post990@gmail.com

            //App Password :
            //twqy lpqr agjc muww