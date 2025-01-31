namespace MeditationCountBot.Services;

public interface IFreeTextMessageSender
{
    Task<List<string>> SendMessageToAllGroups(string message, string mode, CancellationToken cancellationToken);
}