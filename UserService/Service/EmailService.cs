using System.Net.Mail;
using System.Net;

namespace UserService.Service
{
    public class EmailService : IEmailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        public EmailService(IConfiguration configuration)
        {
            _host = configuration["Mailtrap:Host"]; // sandbox.smtp.mailtrap.io
            _port = int.Parse(configuration["Mailtrap:Port"]); // 2525
            _username = configuration["Mailtrap:Username"]; // seu nome de usuário
            _password = configuration["Mailtrap:Password"]; // sua senha
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(_host)
            {
                Port = _port,
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("noreply@seu_dominio.com"), // Altere para um e-mail válido
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
