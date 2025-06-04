namespace TelegramBotFramework.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Telegram.Bot.Types;

    public class PendingQuestionsService : IPendingQuestionsService
    {
        private readonly Dictionary<long, (User User, Question Question)> _pending = new();

        public Task AddQuestion(User user, Question question)
        {
            lock (_pending)
            {
                _pending[user.Id] = (user, question);
            }

            return Task.CompletedTask;
        }

        public Task CheckAnswer(User user, string answer)
        {
            lock (_pending)
            {
                if (_pending.TryGetValue(user.Id, out var pending) &&
                    string.Equals(pending.Question.Answer, answer, StringComparison.OrdinalIgnoreCase))
                {
                    _pending.Remove(user.Id);
                }
            }

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<User>> GetPendingUsers()
        {
            List<User> users;
            lock (_pending)
            {
                users = _pending.Values.Select(v => v.User).ToList();
            }

            return Task.FromResult((IReadOnlyList<User>)users);
        }
    }
}