namespace TelegramBotFramework.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IOpenAiChatService
    {
        Task<string> GetCompletion(string userText, CancellationToken cancellationToken = default);
    }
}
