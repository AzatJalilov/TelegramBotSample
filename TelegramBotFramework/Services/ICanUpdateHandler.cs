namespace TelegramBotFramework.Services
{
    using Telegram.Bot;
    using Telegram.Bot.Types;

    public interface ICanUpdateHandler
    {
        Task<bool> CanHandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken);
        Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        bool StopAfterHandle => true;
    }
}