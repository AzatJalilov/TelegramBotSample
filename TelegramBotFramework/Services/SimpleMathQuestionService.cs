namespace TelegramBotFramework.Services
{
    public class SimpleMathQuestionService : IQuestionService
    {
        private readonly Random _random = new();
        public Question GetNextQuestion() 
        {
            var a = this._random.Next(1, 10);
            var b = this._random.Next(1, 10);
            return new Question
            {
                QuestionText = "What is " + a + " + " + b + "?",
                Answer = (a + b).ToString()
            };
        }
    }
}