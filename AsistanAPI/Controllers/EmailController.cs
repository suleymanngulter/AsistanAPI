using AsistanAPI.Models;
using AsistanAPI.Services.AiService;
using AsistanAPI.Services.EmailService;
using Microsoft.AspNetCore.Mvc;

namespace AsissistanHelper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IAIService _aiService;

        public EmailController(IEmailService emailService, IAIService aiService)
        {
            _emailService = emailService;
            _aiService = aiService;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] AccountParameters account)
        {
            try
            {
                _emailService.CheckEmailSettings(account);
                var emails = _emailService.ReadEmails();
                return Ok(emails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error occurred while connecting to email server: " + ex.Message);
            }
        }

        [HttpPost("Summaries")]
        public async Task<ActionResult<string>> GetSummaries([FromBody] List<Email> selectedEmails)
        {
            if (selectedEmails == null || !selectedEmails.Any())
            {
                return BadRequest("No emails provided or invalid format.");
            }

            try
            {
                var summary = await _aiService.SendPrompt(selectedEmails);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error occurred while generating summary: " + ex.Message);
            }
        }

    }
}
