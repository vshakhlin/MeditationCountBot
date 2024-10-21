using MeditationCountBot.Dto;
using Telegram.Bot.Types;

namespace MeditationCountBot.Services;

public class CounterService : ICounterService
{
    private readonly IJsonLoader _jsonLoader;
    private readonly IJsonLogger _jsonLogger;
    private Dictionary<string, CounterDto> _dictCounters;

    public CounterService(IJsonLoader jsonLoader, IJsonLogger jsonLogger)
    {
        _jsonLoader = jsonLoader;
        _jsonLogger = jsonLogger;
    }

    public async Task Initialize()
    {
        await Reload();
    }
    
    public async Task Reload()
    {
        _dictCounters = await _jsonLoader.LoadAllJsons();
    }

    public async Task CountAndSave(string chatId, string text, User user, DateTime messageDate)
    {
        var counterDto = GetOrCreateCounterDto(chatId);
        var time = TimeParserHelper.ParseTime(text, counterDto.Today);

        if (time != TimeSpan.Zero)
        {
            CountContinuouslyParticipant(counterDto, user, time, messageDate);
            
            await _jsonLogger.Log(counterDto.ChatId, new JsonLog()
            {
                UserId = user.Id,
                MessageDate = messageDate,
                Text = text,
                Time = time,
                TotalTime = counterDto.Today,
            });
            
            if (DateTime.UtcNow.Date == messageDate.Date)
            {
                counterDto.Today += time;
            }
            else
            {
                counterDto.Today = time;
            }
            await _jsonLoader.SaveToJsonAsync(counterDto);
        }
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

    private void CountContinuouslyParticipant(CounterDto counterDto, User user, TimeSpan time, DateTime messageDate)
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