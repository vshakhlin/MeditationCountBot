namespace MeditationCountBot.Services;

public interface ITimeFormatter
{
    string DaysFormat(int days);

    string HoursFormat(int hours);

    string MinutesFormat(int minutes);
}