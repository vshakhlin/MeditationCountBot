namespace MeditationCountBot.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime GetDateTimeUtcNow()
    {
        return DateTime.UtcNow;
    }
    
    public DateTime GetDateTimeNow(TimeSpan timeZone)
    {
        return DateTime.UtcNow.AddHours(timeZone.Hours).AddMinutes(timeZone.Minutes);
    }
    
    public DateTime GetDateTimeWithOffset(DateTime value, TimeSpan timeZone)
    {
        return value.AddHours(timeZone.Hours).AddMinutes(timeZone.Minutes);
    }
}