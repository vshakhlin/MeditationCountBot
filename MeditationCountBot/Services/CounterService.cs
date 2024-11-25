using MeditationCountBot.Dto;
using Telegram.Bot.Types;

namespace MeditationCountBot.Services;

public class CounterService : ICounterService
{
    private readonly IJsonLoader _jsonLoader;
    private readonly IDateTimeService _dateTimeService;
    private Dictionary<string, CounterDto> _dictCounters;

    public CounterService(IJsonLoader jsonLoader, IDateTimeService dateTimeService)
    {
        _jsonLoader = jsonLoader;
        _dateTimeService = dateTimeService;
    }

    public async Task Initialize()
    {
        await Reload();
    }
    
    public async Task Reload()
    {
        _dictCounters = await _jsonLoader.LoadAllJsons<CounterDto>(JsonLoader.ChatsPath);
    }

    public async Task UpdateSettings(string chatId, SettingsDto settings)
    {
        var counterDto = GetOrCreateCounterDto(chatId);
        counterDto.Settings = settings;
        await _jsonLoader.SaveToJsonAsync(chatId, counterDto, JsonLoader.ChatsPath);
    }
    
    public async Task<CounterDto> CountAndSave(string chatId, TimeSpan time, User user, DateTime messageDate)
    {
        var counterDto = GetOrCreateCounterDto(chatId);

        var messageDateTimeZone = _dateTimeService.GetDateTimeWithOffset(messageDate, counterDto.Settings.TimeZone);
        AddTimeToParticipant(counterDto, user, time, messageDateTimeZone);

        counterDto.Today += time;

        await _jsonLoader.SaveToJsonAsync(chatId, counterDto, JsonLoader.ChatsPath);

        return counterDto;
    }
    
    public async Task<CounterDto> ReCountAndSave(string chatId, TimeSpan diffTime, User user)
    {
        var counterDto = GetOrCreateCounterDto(chatId);
        
        var participantDto = counterDto.Participants.FirstOrDefault(p => p.Id == user.Id);
        if (participantDto != null)
        {
            participantDto.Total += diffTime;
        }

        counterDto.Today += diffTime;
        
        await _jsonLoader.SaveToJsonAsync(chatId, counterDto, JsonLoader.ChatsPath);

        return counterDto;
    }

    private CounterDto GetOrCreateCounterDto(string chatId)
    {
        CounterDto counterDto;
        if (_dictCounters.ContainsKey(chatId))
        {
            counterDto = _dictCounters[chatId];
        }
        else
        {
            counterDto = new CounterDto()
            {
                ChatId = chatId,
                Best = TimeSpan.Zero,
                Today = TimeSpan.Zero,
                Yesterday = TimeSpan.Zero,
                Settings = new SettingsDto()
                {
                    TimeZone = TimeSpan.FromHours(3),
                },
                Participants = new List<ParticipantDto>()
            };
            _dictCounters.Add(chatId, counterDto);
        }
        return counterDto;
    }

    private void AddTimeToParticipant(CounterDto counterDto, User user, TimeSpan time, DateTime messageDate)
    {
        var participantDto = counterDto.Participants.FirstOrDefault(p => p.Id == user.Id);
        if (participantDto == null)
        {
            participantDto = new ParticipantDto()
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Total = time,
                ContinuouslyDays = 0,
                LastMeditation = messageDate,
            };
            counterDto.Participants.Add(participantDto);
        }
        else
        {
            participantDto.Total += time;
            participantDto.LastMeditation = messageDate;
        }
    }
}