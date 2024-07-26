using AsistanAPI.Models;

namespace AsistanAPI.Services.EmailService
{
    public interface IEmailService
    {
        void CheckEmailSettings(AccountParameters emailSettings);
        List<Email> ReadEmails();
    }
}
