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

        public async Task SendTourEmail(TourDTO request, string htmlTemplatePath,string contentTemplatePath, Dictionary<string, string> placeholders) 
        {
            try 
            {

                var detailsEmail = new MimeMessage();
                detailsEmail.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                detailsEmail.To.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                detailsEmail.Subject = "Tour Query";

                var contentTemplate = await File.ReadAllTextAsync(contentTemplatePath);

                foreach(var placeholder in placeholders)
                {
                    contentTemplate = contentTemplate.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
                }

                var maloContent = contentTemplate
                    .Replace("{{Name}}", request.Name)
                    .Replace("{{Email}}", request.Email)
                    .Replace("{{TypeOfTour}}", request.TypeOfTour)
                    .Replace("{{PersonOfInterest}}", request.PersonOfInterest)
                    .Replace("{{Timeline}}", request.Timeline)
                    .Replace("{{Feature}}", request.Feature)
                    .Replace("{{DateTime}}", request.DateTime)
                    .Replace("{{Media}}", request.Media);

                var contentBuilder = new BodyBuilder
                {
                    HtmlBody = maloContent
                };

                detailsEmail.Body = contentBuilder.ToMessageBody();

                var confirmEmail = new MimeMessage();
                confirmEmail.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                confirmEmail.To.Add(MailboxAddress.Parse(request.Email));
                confirmEmail.Subject = "Confirmation Email";

                var htmlTemplate = await File.ReadAllTextAsync(htmlTemplatePath);

                foreach (var placeholder in placeholders)
                {
                    htmlTemplate = htmlTemplate.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
                }

                var finalContent = htmlTemplate
                    .Replace("{{UserName}}", request.Name);

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = finalContent

                };

                confirmEmail.Body = bodyBuilder.ToMessageBody();
                

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
                await smtp.SendAsync(detailsEmail);
                await smtp.SendAsync(confirmEmail);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email sent to {request.Email}");

            } catch(Exception ex) 
            {

                _logger.LogError($"Error sending email to {request.Name}: {ex.Message}");
            }
        }

        public async Task SendContactEmail(ContactDTO request, string htmlTemplatePath,string contentTemplatePath , Dictionary<string, string> placeholders)
        {
            try
            {

                var detailsEmail = new MimeMessage();
                detailsEmail.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                detailsEmail.To.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                detailsEmail.Subject = "Contact Query";

                var contentTemplate = await File.ReadAllTextAsync(contentTemplatePath);

                foreach (var placeholder in placeholders)
                {
                    contentTemplate = contentTemplate.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
                }

                var maloContent = contentTemplate
                   .Replace("{{UserName}}", request.Name)
                   .Replace("{{Email}}", request.Email)
                   .Replace("{{Phone}}", request.PhoneNumber)
                   .Replace("{{Reason}}", request.Reason)
                   .Replace("{{Media}}", request.Media)
                   .Replace("{{Query}}", request.Query);

                var contentBuilder = new BodyBuilder
                {
                    HtmlBody = maloContent
                };

                detailsEmail.Body = contentBuilder.ToMessageBody();


                var confirmEmail = new MimeMessage();
                confirmEmail.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
                confirmEmail.To.Add(MailboxAddress.Parse(request.Email));
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

                _logger.LogInformation($"Email sent to {request.Email}");

            }
            catch (Exception ex)
            {

                _logger.LogError($"Error sending email to {request.Name}: {ex.Message}");
            }
        }
    }
}
