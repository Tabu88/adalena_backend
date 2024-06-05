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
        [Route("Send")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendEmail(EmailDTO request)
        {

            var placeholders = new Dictionary<string, string>
            {
                { "Username" , request.Username}
            };

            var htmltemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "EmailConfirmation.html");
            await _emailService.SendEmail(request, htmltemplatePath, placeholders);
            return Ok("Email sent successfully");
        
        }
    }
}
