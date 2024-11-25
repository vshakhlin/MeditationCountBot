namespace MeditationCountBot.Services;

public interface IDateTimeService
{
    DateTime GetDateTimeUtcNow();

    DateTime GetDateTimeNow(TimeSpan timeZone);

    DateTime GetDateTimeWithOffset(DateTime value, TimeSpan timeZone);
}