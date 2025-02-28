using System.Text;
using MeditationCountBot.Dto;
using Telegram.Bot.Extensions;

namespace MeditationCountBot.Services;

public class MessageFormer : IMessageFormer
{
    private readonly ITimeFormatter _timeFormatter;
    private readonly IPraiseAndCheerMessage _praiseAndCheerMessage;
    private readonly IMessageProvider _messageProvider;
    
    public MessageFormer(ITimeFormatter timeFormatter, IPraiseAndCheerMessage praiseAndCheerMessage, IMessageProvider messageProvider)
    {
        _timeFormatter = timeFormatter;
        _praiseAndCheerMessage = praiseAndCheerMessage;
        _messageProvider = messageProvider;
    }

    public string CreateMessage(CounterDto counterDto)
    {
        var sb = new StringBuilder(
            $"{_messageProvider.TogetherTimeMessage} {counterDto.Today.TotalMinutes} \\({counterDto.Today.Hours} {_timeFormatter.HoursFormat(counterDto.Today.Hours)} {counterDto.Today.Minutes} {_timeFormatter.MinutesFormat(counterDto.Today.Minutes)}\\)");

        if (counterDto.Yesterday != TimeSpan.Zero)
        {
            if (counterDto.Today > counterDto.Best)
            {
                sb.Append($"\nПоздравляю\\! У нас новый рекорд {counterDto.Today.TotalMinutes}");
            }
            else if (counterDto.Today > counterDto.Yesterday)
            {
                sb.Append(
                    $"\nНа {counterDto.Today.TotalMinutes - counterDto.Yesterday.TotalMinutes} лучше чем вчера\\. {_praiseAndCheerMessage.GetRandomPraiseMessage()}");
            }
            else if (counterDto.Today < counterDto.Yesterday)
            {
                sb.Append(
                    $"\nНа {counterDto.Yesterday.TotalMinutes - counterDto.Today.TotalMinutes} меньше чем вчера\\. {_praiseAndCheerMessage.GetRandomCheerMessage()}");
            }
        }

        int lastDays = int.MaxValue;
        foreach (var participantDto in counterDto.Participants.OrderByDescending(p => p.ContinuouslyDays))
        {
            if (participantDto.ContinuouslyDays <= 1)
            {
                break;
            }
            if (lastDays != participantDto.ContinuouslyDays)
            {
                lastDays = participantDto.ContinuouslyDays;
                sb.Append($"\n\n{_messageProvider.ContinueMessage}{lastDays} {_timeFormatter.DaysFormat(lastDays)} подряд:");    
            }
            sb.Append($"\n \\- {FormatName(participantDto)}"); 
            
        }

        return sb.ToString();
    }
    
    private string FormatName(ParticipantDto participantDto)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(participantDto.FirstName))
        {
            sb.Append(participantDto.FirstName);
        }
        
        if (!string.IsNullOrEmpty(participantDto.LastName))
        {
            sb.Append(' ').Append(participantDto.LastName);
        }
        
        if (!string.IsNullOrEmpty(participantDto.Username))
        {
            sb.Append(" (@").Append(participantDto.Username).Append(")");
        }

        return Markdown.Escape(sb.ToString());
    }
    
}