namespace MeditationCountBot.Dto;

public class CounterDto
{
    public string ChatId { get; set; }

    public TimeSpan Best { get; set; }
    
    public TimeSpan Yesterday { get; set; }
    
    public TimeSpan Today { get; set; }
    
    public TimeSpan Total { get; set; }

    public SettingsDto Settings { get; set; }

    public List<ParticipantDto> Participants { get; set; }
}