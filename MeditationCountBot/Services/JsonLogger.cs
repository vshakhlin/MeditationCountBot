using System.Net;
using System.Text.Json;
using MeditationCountBot.Dto;

namespace MeditationCountBot.Services;

public class JsonLogger : IJsonLogger
{
    public async Task Log(string chatId, JsonLog jsonLog)
    {
        await File.AppendAllTextAsync($"../logs/{chatId}-{DateTime.Today:yyyy-MM-DD}.json",  $"{JsonSerializer.Serialize(jsonLog)},{Environment.NewLine}");
    }
}