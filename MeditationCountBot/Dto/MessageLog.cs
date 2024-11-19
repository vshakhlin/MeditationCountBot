namespace MeditationCountBot.Dto;

public class MessageLog
{
    public long UserId { get; set; }
    public int MessageId { get; set; }
    public DateTime MessageDate { get; set; }
    public TimeSpan Time { get; set; }
    public string Text { get; set; }
    public TimeSpan TotalTime { get; set; }
}