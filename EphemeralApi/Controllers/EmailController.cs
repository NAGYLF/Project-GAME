using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Mvc;

[Route("api/email")]
[ApiController]
public class EmailController : ControllerBase
{
    // E-mail küldése
    [HttpPost]
    public IActionResult SendEmail([FromBody] EmailRequest request)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Veller Árpád", "ephemeralcourage@gmail.com"));
            message.To.Add(new MailboxAddress("Címzett", request.To));
            message.Subject = request.Subject;

            // E-mail törzsének beállítása HTML formátumban
            message.Body = new TextPart("html")
            {
                Text = request.Body
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("ephemeralcourage@gmail.com", "mrkj yclq zkhr gyiq");
                client.Send(message);
                client.Disconnect(true);
            }

            return Ok($"Email elküldve {request.To} címre!");
        }
        catch (Exception ex)
        {
            return BadRequest($"Hiba: {ex.Message}");
        }
    }
}

// DTO az e-mail kéréshez
public class EmailRequest
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}