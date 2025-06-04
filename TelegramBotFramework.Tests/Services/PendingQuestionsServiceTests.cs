using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotFramework.Services;
using Xunit;

namespace TelegramBotFramework.Tests.Services
{
    public class PendingQuestionsServiceTests
    {
        [Fact]
        public async Task AddQuestion_StoresPendingUser()
        {
            var service = new PendingQuestionsService();
            var user = new User { Id = 1, FirstName = "Test" };
            var question = new Question { QuestionText = "1+1", Answer = "2" };

            await service.AddQuestion(user, question);

            var pending = await service.GetPendingUsers();
            Assert.Contains(pending, u => u.Id == user.Id);
        }

        [Fact]
        public async Task CheckAnswer_RemovesUserOnCorrectAnswer()
        {
            var service = new PendingQuestionsService();
            var user = new User { Id = 1, FirstName = "Test" };
            var question = new Question { QuestionText = "1+1", Answer = "2" };

            await service.AddQuestion(user, question);
            await service.CheckAnswer(user, "2");

            var pending = await service.GetPendingUsers();
            Assert.DoesNotContain(pending, u => u.Id == user.Id);
        }
    }
}
