using System.Text.Json;
using MeditationCountBot.Dto;
using MeditationCountBot.Services;

namespace MeditationCountBot.Tests;

public class TimeFormatterTest
{
    [Fact]
    public void DaysTest()
    {
        var timeFormatter = new TimeFormatter();
        Assert.Equal("день", timeFormatter.DaysFormat(1));
        Assert.Equal("дня", timeFormatter.DaysFormat(2));
        Assert.Equal("дня", timeFormatter.DaysFormat(3));
        Assert.Equal("дня", timeFormatter.DaysFormat(4));
        for (var i = 5; i <= 20; i++)
        {
            Assert.Equal("дней", timeFormatter.DaysFormat(i));
        }
        
        Assert.Equal("день", timeFormatter.DaysFormat(21));
        Assert.Equal("дня", timeFormatter.DaysFormat(22));
        Assert.Equal("дня", timeFormatter.DaysFormat(23));
        Assert.Equal("дня", timeFormatter.DaysFormat(24));
        
        for (var i = 25; i <= 30; i++)
        {
            Assert.Equal("дней", timeFormatter.DaysFormat(i));
        }
        
        Assert.Equal("дней", timeFormatter.DaysFormat(100));
        Assert.Equal("день", timeFormatter.DaysFormat(101));
        Assert.Equal("дня", timeFormatter.DaysFormat(102));
        Assert.Equal("дня", timeFormatter.DaysFormat(103));
        Assert.Equal("дня", timeFormatter.DaysFormat(104));
        Assert.Equal("дней", timeFormatter.DaysFormat(105));
    }
    
    [Fact]
    public void HoursTest()
    {
        var timeFormatter = new TimeFormatter();
        Assert.Equal("час", timeFormatter.HoursFormat(1));
        Assert.Equal("часа", timeFormatter.HoursFormat(2));
        Assert.Equal("часа", timeFormatter.HoursFormat(3));
        Assert.Equal("часа", timeFormatter.HoursFormat(4));
        for (var i = 5; i <= 20; i++)
        {
            Assert.Equal("часов", timeFormatter.HoursFormat(i));
        }
        
        Assert.Equal("час", timeFormatter.HoursFormat(21));
        Assert.Equal("часа", timeFormatter.HoursFormat(22));
        Assert.Equal("часа", timeFormatter.HoursFormat(23));
        Assert.Equal("часа", timeFormatter.HoursFormat(24));
        
        for (var i = 25; i <= 30; i++)
        {
            Assert.Equal("часов", timeFormatter.HoursFormat(i));
        }
        
        Assert.Equal("часов", timeFormatter.HoursFormat(100));
        Assert.Equal("час", timeFormatter.HoursFormat(101));
        Assert.Equal("часа", timeFormatter.HoursFormat(102));
        Assert.Equal("часа", timeFormatter.HoursFormat(103));
        Assert.Equal("часа", timeFormatter.HoursFormat(104));
        Assert.Equal("часов", timeFormatter.HoursFormat(105));
    }
    
    [Fact]
    public void MinutesTest()
    {
        var timeFormatter = new TimeFormatter();
        Assert.Equal("минута", timeFormatter.MinutesFormat(1));
        Assert.Equal("минуты", timeFormatter.MinutesFormat(2));
        Assert.Equal("минуты", timeFormatter.MinutesFormat(3));
        Assert.Equal("минуты", timeFormatter.MinutesFormat(4));
        for (var i = 5; i <= 20; i++)
        {
            Assert.Equal("минут", timeFormatter.MinutesFormat(i));
        }
        
        Assert.Equal("минута", timeFormatter.MinutesFormat(21));
        Assert.Equal("минуты", timeFormatter.MinutesFormat(22));
        Assert.Equal("минуты", timeFormatter.MinutesFormat(23));
        Assert.Equal("минуты", timeFormatter.MinutesFormat(24));
        
        for (var i = 25; i <= 30; i++)
        {
            Assert.Equal("минут", timeFormatter.MinutesFormat(i));
        }
        
        Assert.Equal("минут", timeFormatter.MinutesFormat(100));
        Assert.Equal("минута", timeFormatter.MinutesFormat(101));
        Assert.Equal("минуты", timeFormatter.MinutesFormat(102));
        Assert.Equal("минуты", timeFormatter.MinutesFormat(103));
        Assert.Equal("минуты", timeFormatter.MinutesFormat(104));
        Assert.Equal("минут", timeFormatter.MinutesFormat(105));
    }
}