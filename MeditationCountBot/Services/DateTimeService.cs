namespace MeditationCountBot.Services;

public class DateTimeService : IDateTimeService
{
    private int _timeZoneOffsetHours; 
    
    public void Initialize(int timeZoneOffsetHours)
    {
        _timeZoneOffsetHours = timeZoneOffsetHours;
    }

    public DateTime GetDateTimeUtcNow()
    {
        return DateTime.UtcNow;
    }
    
    public DateTime GetDateTimeNow()
    {
        return DateTime.UtcNow.AddHours(_timeZoneOffsetHours);
    }
    
    public DateTime GetDateTimeWithOffset(DateTime value)
    {
        return value.AddHours(_timeZoneOffsetHours);
    }
}