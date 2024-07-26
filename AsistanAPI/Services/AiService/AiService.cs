using AsistanAPI.Models;
using AsistanAPI.Services.AiService;
using AsistanAPI.Services.EmailService;
using OpenAI_API;
using OpenAI_API.Chat;
using System.Text;

public class AIService : IAIService
{
    private readonly IEmailService _emailService;
    private readonly string apiKey = "enterApikey"; // Ideally, load from configuration
    private readonly string model = "gpt-4o-mini";
    string mesaj = "sen türkçe bilen bir asistansın. çalıştığın şirkette sürekli işle ilgili mailler alan bir yöneticin var ına maillerinin özetini sunman gerek";

    public AIService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task<string> SendPrompt(List<Email> selectedEmails)
    {
        StringBuilder promptBuilder = new StringBuilder();

        foreach (var email in selectedEmails)
        {
            promptBuilder.AppendLine("To: " + email.From);
            promptBuilder.AppendLine("Subject: " + email.Subject);
            promptBuilder.AppendLine("Body: " + email.Body);
            promptBuilder.AppendLine(); // Add an empty line to separate emails
        }

        string prompt = promptBuilder.ToString().Trim()+mesaj;

        var client = new OpenAIAPI(apiKey);

        var request = new ChatRequest
        {
            Model = model,
            Messages = new List<ChatMessage>
            {
                new ChatMessage(ChatMessageRole.User, prompt)
            }
        };

        var result = await client.Chat.CreateChatCompletionAsync(request);

        string answer = result.Choices.FirstOrDefault()?.Message.TextContent.Trim() ?? string.Empty;

        return answer;
    }
}
