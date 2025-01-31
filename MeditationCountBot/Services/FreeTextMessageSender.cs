using MeditationCountBot.Dto;
using MeditationCountBot.Telegram;

namespace MeditationCountBot.Services;

public class FreeTextMessageSender(ITelegramMessageSender messageSender, IJsonLoader jsonLoader) : IFreeTextMessageSender
{
    public async Task<List<string>> SendMessageToAllGroups(string message, string mode, CancellationToken cancellationToken)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(message))
        {
            return result;
        }

        var dict = await jsonLoader.LoadAllJsons<CounterDto>(JsonLoader.ChatsPath);
        foreach (var counter in dict.Values)
        {
            if (mode == "test" && counter.ChatId != "-455396485")
            {
                continue;
            }
            await messageSender.SendMessage(counter.ChatId, message, "ru", null, cancellationToken);
            result.Add(counter.ChatId);
        }

        return result;
    }
}