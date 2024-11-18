namespace TelegramBotFramework
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Services;
    using Telegram.Bot;
    using Telegram.Bot.Polling;
    using Telegram.Bot.Types;

    public class TelegramPollingJob(
        ITelegramBotClient botClient,
        IReadOnlyList<ICanUpdateHandler> handlers,
        IOptions<ReceiverOptions> receiverOptions,
        ILogger logger)
        : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Job started");
            botClient.StartReceiving(
                this.HandleUpdateAsync,
                this.HandleErrorAsync,
                receiverOptions.Value,
                cancellationToken
            );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Job started");
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var handler in handlers)
                {
                    if (!await handler.CanHandleUpdateAsync(telegramBotClient, update, cancellationToken))
                    {
                        continue;
                    }

                    await handler.HandleUpdateAsync(telegramBotClient, update, cancellationToken);
                    if (handler.StopAfterHandle)
                    {
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                logger.LogError("Handling update failed {exception}", exception);
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient telegramBotClient, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError("Error happened {exception}", exception);
            return Task.CompletedTask;
        }
    }
}