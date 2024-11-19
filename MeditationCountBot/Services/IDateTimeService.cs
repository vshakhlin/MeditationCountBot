namespace MeditationCountBot.Services;

public interface IDateTimeService
{
    void Initialize(int timeZoneOffsetHours);

    DateTime GetDateTimeUtcNow();

    DateTime GetDateTimeNow();

    DateTime GetDateTimeWithOffset(DateTime value);
}