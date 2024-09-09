using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MovingPapa
{
    internal class EmailService
    {
        private readonly string SmtpServer, Email, Password;
        public readonly string HelpEmail, TechnicalHelpEmail;

        public EmailService () : this (
            //"smtp.gmail.com", "temporary.stibc.verifier@gmail.com", "", "hello@movingpapa.com", "hello@movingpapa.com"
             "smtp-mail.outlook.com", "hello@movingpapa.com", Environment.GetEnvironmentVariable("MOVINGPAPA_EMAIL_PASSWORD"), "hello@movingpapa.com", "hello@movingpapa.com"
        )
        { }

        public EmailService (string smtpServer, string email, string password, string helpEmail, string technicalHelpEmail)
        {
            SmtpServer = smtpServer;
            Email = email;
            Password = password;
            HelpEmail = helpEmail;
            TechnicalHelpEmail = technicalHelpEmail;
        }

        public async Task SendMessage(string to, string subject, string body, IEnumerable<(string name, Stream stream)>? files = null)
        {
            using (SmtpClient smtp = new())
            {
                await smtp.ConnectAsync(host: SmtpServer, port: 587, options: SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(Email, Password);
                BodyBuilder bodyBuilder = new() { HtmlBody = body };
                if (files != null)
                    foreach (var attachment in files)
                        bodyBuilder.Attachments.Add(attachment.name, attachment.stream);
                await smtp.SendAsync(new()
                {
                    From = { MailboxAddress.Parse(Email) },
                    To = { MailboxAddress.Parse(to) },
                    Subject = subject,
                    Body = bodyBuilder.ToMessageBody()
                });
                await smtp.DisconnectAsync(true);
            }
            if (files != null)
                foreach ((_, Stream stream) in files) stream.Close();
        }
    }
}
