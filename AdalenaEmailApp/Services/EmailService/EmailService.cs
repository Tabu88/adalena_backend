using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace AdalenaEmailApp.Services.EmailService
{
    public class EmailService 
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmail(EmailDTO request, string htmlTemplatePath, Dictionary<string, string> placeholders) 
        {
            try 
            {

                var detailsEmail = new MimeMessage();
                detailsEmail.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                detailsEmail.To.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                detailsEmail.Subject = request.Subject;
                detailsEmail.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = request.Body };

                var confirmEmail = new MimeMessage();
                confirmEmail.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                confirmEmail.To.Add(MailboxAddress.Parse(request.To));
                confirmEmail.Subject = "Confirmation Email";

                var htmlTemplate = await File.ReadAllTextAsync(htmlTemplatePath);


                foreach (var placeholder in placeholders)
                {
                    htmlTemplate = htmlTemplate.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
                }

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlTemplate

                };
                confirmEmail.Body = bodyBuilder.ToMessageBody();
                //new TextPart(MimeKit.Text.TextFormat.Html) { htmlTemplatePath };


                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
                await smtp.SendAsync(detailsEmail);
                await smtp.SendAsync(confirmEmail);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email sent to {request.To} with subject {request.Subject}");

            } catch(Exception ex) 
            {

                _logger.LogError($"Error sending email to {request.Body}: {ex.Message}");
            }
        }
    }
}
