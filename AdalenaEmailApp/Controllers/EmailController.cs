using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdalenaEmailApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        [Route("SendContact")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendContactEmail(ContactDTO request)
        {
            var placeholders = new Dictionary<string, string>
            {
                { "Username" , request.Name}
            };

            var contentTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "ContactEmail.html");
            var htmlTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "EmailConfirmation.html");
            await _emailService.SendContactEmail(request, htmlTemplatePath, contentTemplatePath , placeholders);
            return Ok("Email sent successfully");
        
        }

        [HttpPost]
        [Route("SendTour")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendTourEmail(TourDTO request)
        {

            var placeholders = new Dictionary<string, string>
            {
                { "Username" , request.Name}
            };

            var contentTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "TourEmail.html");
            var htmltemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "EmailConfirmation.html");
            await _emailService.SendTourEmail(request, htmltemplatePath, contentTemplatePath, placeholders);
            return Ok("Email sent successfully");

        }
    }
}
