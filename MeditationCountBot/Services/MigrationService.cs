using MeditationCountBot.Dto;

namespace MeditationCountBot.Services;

public class MigrationService : IMigrationService
{
    private readonly IJsonLoader _jsonLoader;

    public MigrationService(IJsonLoader jsonLoader)
    {
        _jsonLoader = jsonLoader;
    }

    public async Task Migrate()
    {
        var dict = await _jsonLoader.LoadAllJsons<CounterDto>(JsonLoader.ChatsPath);
        foreach (var counter in dict.Values)
        {
            if (counter.Settings == null)
            {
                counter.Settings = new SettingsDto()
                {
                    TimeZone = TimeSpan.FromHours(3),
                };
            }

            if (counter.Total == TimeSpan.Zero)
            {
                foreach (var participant in counter.Participants)
                {
                    counter.Total += participant.Total;
                }
            }
            
            await _jsonLoader.SaveToJsonAsync(counter.ChatId, counter, JsonLoader.ChatsPath, false);
        }
    }
}