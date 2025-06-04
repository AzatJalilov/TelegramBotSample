using System.Text.RegularExpressions;
using TelegramBotFramework.Services;
using Xunit;

namespace TelegramBotFramework.Tests.Services;

public class SimpleMathQuestionServiceTests
{
    [Fact]
    public void GetNextQuestion_ReturnsValidAdditionQuestion()
    {
        var service = new SimpleMathQuestionService();
        var question = service.GetNextQuestion();

        // question text like "What is a + b?"
        var match = Regex.Match(question.QuestionText, @"What is (\d+) \+ (\d+)\?");
        Assert.True(match.Success, $"Unexpected question format: {question.QuestionText}");
        var a = int.Parse(match.Groups[1].Value);
        var b = int.Parse(match.Groups[2].Value);
        Assert.Equal((a + b).ToString(), question.Answer);
    }
}
