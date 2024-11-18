namespace TelegramBotFramework.Services
{
    using Telegram.Bot.Types;

    public interface IPendingQuestionsService
    {
        public Task AddQuestion(User user, Question question);
        
        public Task CheckAnswer(User user, string answer);
        
        public Task<IReadOnlyList<User>> GetPendingUsers();
    }
}