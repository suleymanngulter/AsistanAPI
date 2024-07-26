using AsistanAPI.Models;
using OpenAI_API;

namespace AsistanAPI.Services.AiService
{
    public interface IAIService
    {
        public Task<string> SendPrompt(List<Email> sended);
    }
}
