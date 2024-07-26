using AsistanAPI.Services.EmailService;
using AsistanAPI.Models;
using MailKit.Net.Pop3;
using MailKit.Security;
using Org.BouncyCastle.Asn1.Cms;

namespace AsistanAPI.Services.EmailService
{

    public class EmailService : IEmailService
    {
        private AccountParameters _emailSettings;
        internal static List<Email> mailbox = new List<Email>();

        public EmailService()
        {
            _emailSettings = new AccountParameters();
        }

        public void CheckEmailSettings(AccountParameters emailSettings)
        {
            _emailSettings.Pop3Host = emailSettings.Pop3Host;
            _emailSettings.Username = emailSettings.Username;
            _emailSettings.Password = emailSettings.Password;
            _emailSettings.Time = emailSettings.Time;
        }

        public List<Email> ReadEmails()
        {
            if (string.IsNullOrEmpty(_emailSettings.Username))
                throw new ArgumentNullException(nameof(_emailSettings.Username), "EmailUsername is null or empty.");
            if (string.IsNullOrEmpty(_emailSettings.Pop3Host))
                throw new ArgumentNullException(nameof(_emailSettings.Pop3Host), "Pop3Host is null or empty.");
            if (string.IsNullOrEmpty(_emailSettings.Password))
                throw new ArgumentNullException(nameof(_emailSettings.Password), "EmailPassword is null or empty.");

            using (var pop3 = new Pop3Client())
            {
                pop3.Connect(_emailSettings.Pop3Host, 110, SecureSocketOptions.StartTls);
                pop3.Authenticate(_emailSettings.Username, _emailSettings.Password);

                var count = pop3.Count;
                var emails = new List<Email>();

                for (int i = 0; i < count; i++)
                {
                    var message = pop3.GetMessage(i);

                    if (message.Date.Date >= _emailSettings.Time.Date)
                    {
                        emails.Add(new Email
                        {
                            From = string.Join(", ", message.From.Select(mb => mb.ToString())),
                            Subject = message.Subject,
                            Body = !string.IsNullOrEmpty(message.TextBody) ? message.TextBody : message.HtmlBody
                            
                        });
                    }
                }

                mailbox = emails;
                pop3.Disconnect(true);

                return emails;
            }
        }

    }

}
