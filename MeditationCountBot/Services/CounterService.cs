using MeditationCountBot.Dto;
using Telegram.Bot.Types;

namespace MeditationCountBot.Services;

public class CounterService : ICounterService
{
    private readonly IJsonLoader _jsonLoader;
    private Dictionary<string, CounterDto> _dictCounters;

    public CounterService(IJsonLoader jsonLoader)
    {
        _jsonLoader = jsonLoader;
    }

    public async Task Initialize()
    {
        await Reload();
    }
    
    public async Task Reload()
    {
        _dictCounters = await _jsonLoader.LoadAllJsons<CounterDto>(JsonLoader.ChatsPath);
    }

    public async Task<CounterDto> CountAndSave(string chatId, TimeSpan time, User user, DateTime messageDate)
    {
        var counterDto = GetOrCreateCounterDto(chatId);

        AddTimeToParticipant(counterDto, user, time, messageDate);

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