using MeditationCountBot.Dto;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MeditationCountBot.Services;

public class ReCalculateResultService(
    IJsonLoader jsonLoader,
    IDateTimeService dateTimeService,
    ILogReader logReader,
    ILogger<CalculateResultService> logger)
    : IReCalculateResultService
{
    public async Task<List<string>> ReCalculate(string logFile, DateTime date)
    {
        logger.LogInformation($"Start recalculation {dateTimeService.GetDateTimeUtcNow()}");

        var updateMessages = logReader.ReadLogs(LogReader.LogsPath, logFile);
        
        var results = new List<string>();
        var backupDict = await jsonLoader.LoadAllJsons<CounterDto>(JsonLoader.BackupPath);
        var dict = await jsonLoader.LoadAllJsons<CounterDto>(JsonLoader.ChatsPath);
        var tempParticipantDict = new Dictionary<string, DateTime>();
        logger.LogInformation($"Count: {dict.Count}");
        foreach (var counter in dict.Values)
        {
            if (!backupDict.TryGetValue(counter.ChatId, out var backupCounter))
            {
                continue;
            }

            foreach (var counterParticipant in counter.Participants)
            {
                var backupParticipant = backupCounter.Participants.FirstOrDefault(p => p.Id == counterParticipant.Id);
                if (backupParticipant != null)
                {
                    if (backupParticipant.ContinuouslyDays <= 0)
                    {
                        continue;
                    }
                    if (counterParticipant.ContinuouslyDays - 1 == backupParticipant.ContinuouslyDays)
                    {
                        continue;
                    }

                    if (backupParticipant.LastMeditation > date)
                    {
                        tempParticipantDict.Add($"{counter.ChatId}_{counterParticipant.Id}",
                            backupParticipant.LastMeditation);
                    }

                    counterParticipant.LastMeditation = backupParticipant.LastMeditation;
                    counterParticipant.ContinuouslyDays = backupParticipant.ContinuouslyDays;
                    counterParticipant.BestContinuouslyDays = backupParticipant.BestContinuouslyDays;
                }

                var updatedMessages = updateMessages.Where(m =>
                    m.Type == UpdateType.Message && 
                    m.Message.Chat.Id.ToString() == counter.ChatId &&
                    m.Message.From.Id == counterParticipant.Id && m.Message.Date < date).ToList();
                
                
                var editedMessages = updateMessages.Where(m =>
                    m.Type == UpdateType.EditedMessage && 
                    m.EditedMessage.Chat.Id.ToString() == counter.ChatId &&
                     m.EditedMessage.From.Id == counterParticipant.Id && m.EditedMessage.Date < date).ToList();

                foreach (var message in updatedMessages)
                {
                    ParseTextMessage(counterParticipant, message.Message, date);
                }
                
                foreach (var message in editedMessages)
                {
                    ParseTextMessage(counterParticipant, message.EditedMessage, date);
                }
                
                if (counterParticipant.LastMeditation.Date == date.AddDays(-1).Date)
                {
                    counterParticipant.ContinuouslyDays += 1;
                }
            }

            // calculateContinuouslyService.CalculateContinuouslyDays(counter, date);

            // restore last time meditation
            foreach (var counterParticipant in counter.Participants)
            {
                if (tempParticipantDict.TryGetValue($"{counter.ChatId}_{counterParticipant.Id}", out var lastTime))
                {
                    counterParticipant.LastMeditation = lastTime;
                }
            }

            
            await jsonLoader.SaveToJsonAsync(counter.ChatId, counter, "../newchats", false);
            
            results.Add(counter.ChatId);
        }
        
        // await _counterService.Reload();

        return results;
    }

    private void ParseTextMessage(ParticipantDto counterParticipant, Message message, DateTime date)
    {
        if (message.Date > date)
        {
            return;
        }

        var text = message.Text;
        if (string.IsNullOrEmpty(message.Text))
        {
            text = message.Caption;
        }

        var time = TimeParserHelper.ParseTime(text);

        if (time != TimeSpan.Zero)
        {
            counterParticipant.LastMeditation = message.Date;
        }
    }
}