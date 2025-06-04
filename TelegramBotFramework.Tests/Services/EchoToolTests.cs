using TelegramBotFramework.Services;
using Xunit;

namespace TelegramBotFramework.Tests.Services;

public class EchoToolTests
{
    [Fact]
    public void Echo_ReturnsSameMessage()
    {
        var message = "hello";
        var result = EchoTool.Echo(message);
        Assert.Equal(message, result);
    }
}
