using MeditationCountBot.Services;

namespace MeditationCountBot.Tests;

public class PraiseAndCheerMessageTests
{
    [Fact]
    public void GetRandomPraiseMessageTest()
    {
        var praiseAndCheerMessage = new PraiseAndCheerMessage();
        var message = praiseAndCheerMessage.GetRandomPraiseMessage();
        Assert.NotNull(message);
    }
    
    [Fact]
    public void GetRandomCheerMessageTest()
    {
        var praiseAndCheerMessage = new PraiseAndCheerMessage();
        var message = praiseAndCheerMessage.GetRandomCheerMessage();
        Assert.NotNull(message);
    }
}