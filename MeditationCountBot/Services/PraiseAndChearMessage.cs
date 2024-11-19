namespace MeditationCountBot.Services;

public class PraiseAndCheerMessage : IPraiseAndCheerMessage
{
    private static readonly List<string> PraiseMessages = new List<string>() { "Так держать\\! ❤️", "Вы лучшие\\! ❤️", "Продолжайте в том же духе\\! ❤️", "Вы на верном пути\\! ❤️" };
    private static readonly List<string> CheerMessages = new List<string>() { "Не сдавайтесь\\! 🙏", "Главное стабильность\\! 🙏", "Просто продолжайте\\! 🙏" };

    public string GetRandomPraiseMessage()
    {
        return PraiseMessages[GetRandomIndex(PraiseMessages.Count)];
    }
    
    public string GetRandomCheerMessage()
    {
        return CheerMessages[GetRandomIndex(CheerMessages.Count)];
    }

    private int GetRandomIndex(int maxCount)
    {
        Random rnd = new Random();
        return rnd.Next(0, maxCount);
    }
}