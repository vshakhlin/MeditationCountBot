namespace MeditationCountBot.Dto;

public class ParticipantDto
{
    public long Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Username { get; set; }
    public TimeSpan Total { get; set; }
    public int ContinuouslyDays { get; set; }
    public DateTime LastMeditation { get; set; }
}