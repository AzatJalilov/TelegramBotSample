# TelegramBotSample

This repository contains a sample Telegram bot built with ASP.NET Core and the Telegram.Bot library.

## Features
- Polls Telegram for updates using long polling
- Sends simple math questions to new chat members
- Stores pending questions in memory
- Includes basic unit tests

## Getting started
1. Install the .NET 8 SDK.
2. Set your bot token in the `BotToken` configuration value or as an environment variable.
3. Run the bot:
   ```bash
   dotnet run --project TelegramBot.Web
   ```
4. Interact with the bot in Telegram.

## Testing
Run the tests with:
```bash
dotnet test
```
