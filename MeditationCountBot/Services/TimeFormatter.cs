namespace MeditationCountBot.Services;

public class TimeFormatter : ITimeFormatter
{
    public string DaysFormat(int days)
    {
        return TimeUnitFormat(days, DayFormat);
    }
    
    public string HoursFormat(int hours)
    {
        return TimeUnitFormat(hours, HourFormat);
    }
    
    public string MinutesFormat(int minutes)
    {
        return TimeUnitFormat(minutes, MinuteFormat);
    }
    
    private string TimeUnitFormat(int timeUnit, Func<int, string> formatter)
    {
        if (timeUnit > 4)
        {
            if (timeUnit > 20)
            {
                var lastDigit = timeUnit % 10;
                return formatter(lastDigit);
            }
        }

        return formatter(timeUnit);
    }

    private string DayFormat(int days)
    {
        switch (days)
        {
            case 1:
                return "день";
            case 2:
                return "дня";
            case 3:
                return "дня";
            case 4:
                return "дня";
            default:
                return "дней";
        }
    }
    
    private string HourFormat(int hours)
    {
        switch (hours)
        {
            case 1:
                return "час";
            case 2:
                return "часа";
            case 3:
                return "часа";
            case 4:
                return "часа";
            default:
                return "часов";
        }
    }
    
    private string MinuteFormat(int minutes)
    {
        switch (minutes)
        {
            case 1:
                return "минута";
            case 2:
                return "минуты";
            case 3:
                return "минуты";
            case 4:
                return "минуты";
            default:
                return "минут";
        }
    }
}