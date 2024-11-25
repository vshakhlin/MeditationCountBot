using MeditationCountBot.Services;

namespace MeditationCountBot.Tests;

public class DateTimeServiceTests
{
    [Fact]
    public void DateTimeUtcNowTest()
    {
        var dateTimeService = new DateTimeService();
        var utcNow = dateTimeService.GetDateTimeUtcNow();
        Assert.Equal(DateTime.UtcNow.Date, utcNow.Date);
        Assert.Equal(DateTime.UtcNow.Hour, utcNow.Hour);
        Assert.Equal(DateTime.UtcNow.Minute, utcNow.Minute);
        Assert.Equal(DateTime.UtcNow.Second, utcNow.Second);
    }
    
    [Fact]
    public void DateTimeNowTest()
    {
        var dateTimeService = new DateTimeService();
        var now = dateTimeService.GetDateTimeNow(TimeSpan.FromHours(3));
        var expectNow = DateTime.UtcNow.AddHours(3);
        Assert.Equal(expectNow.Date, now.Date);
        Assert.Equal(expectNow.Hour, now.Hour);
        Assert.Equal(expectNow.Minute, now.Minute);
        Assert.Equal(expectNow.Second, now.Second);
    }
    
    [Fact]
    public void DateTimeWithOffsetTest()
    {
        var dateTimeService = new DateTimeService();
        var now = dateTimeService.GetDateTimeWithOffset(DateTime.Parse("2024-07-24T09:38:57Z"), TimeSpan.FromHours(3));
        Assert.Equal(2024, now.Year);
        Assert.Equal(7, now.Month);
        Assert.Equal(24, now.Day);
        Assert.Equal(12, now.Hour);
        Assert.Equal(38, now.Minute);
        Assert.Equal(57, now.Second);
    }
    
    [Fact]
    public void DateTimeWithOffsetNextDayTest()
    {
        var dateTimeService = new DateTimeService();
        var now = dateTimeService.GetDateTimeWithOffset(DateTime.Parse("2024-07-24T23:38:57Z"), TimeSpan.FromHours(3));
        Assert.Equal(2024, now.Year);
        Assert.Equal(7, now.Month);
        Assert.Equal(25, now.Day);
        Assert.Equal(2, now.Hour);
        Assert.Equal(38, now.Minute);
        Assert.Equal(57, now.Second);
    }
}