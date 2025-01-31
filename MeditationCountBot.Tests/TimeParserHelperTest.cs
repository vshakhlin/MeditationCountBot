using MeditationCountBot.Services;

namespace MeditationCountBot.Tests;

public class TimeParserHelperTest
{
    [Fact]
    public void UrlTest()
    {
        // 20 - 0 = +20
        // 15 - 5 = +10
        
        // 15 - 20 = -5
        // 45 - 445 = -
        // 0 - 20 = -20
        var total = TimeSpan.FromMinutes(60);
        var diff = TimeSpan.FromMinutes(15) - TimeSpan.FromMinutes(20);
        var sum = total + diff;
        Assert.Equal(sum, TimeSpan.FromMinutes(55));
            
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("https://mechtyumorya.ru/?ysclid=lzdzebsuue+326785678"));
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("mechtyumorya.ru/?ysclid=lzdzebsuue+326785678"));
    }
    
    [Fact]
    public void NoTimeTest()
    {
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("В Алмате тоже есть горы)"));
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("Экспедиция на Шри Ланку для своих с 5 по 13 декабря"));
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("Ребята! Поднажмем!! Уже до 500 не доползаем!!"));
    }
    
    [Fact]
    public void BigInt()
    {
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("Запись по телефону: [+79098227286]"));
    }
    
    [Fact]
    public void StandartTest()
    {
        Assert.Equal(TimeSpan.FromMinutes(20), TimeParserHelper.ParseTime("+20 = 415"));
        Assert.Equal(TimeSpan.FromMinutes(30), TimeParserHelper.ParseTime("+30= 380"));
        Assert.Equal(TimeSpan.FromMinutes(15), TimeParserHelper.ParseTime("+15\n501"));
    }
    
    [Fact]
    public void ShortTest()
    {
        Assert.Equal(TimeSpan.FromMinutes(25), TimeParserHelper.ParseTime("+25🙏"));
        Assert.Equal(TimeSpan.FromMinutes(15), TimeParserHelper.ParseTime("+15"));
        Assert.Equal(TimeSpan.FromMinutes(25), TimeParserHelper.ParseTime("+25🙏"));
    }
    
    [Fact]
    public void WithSpaceTest()
    {
        Assert.Equal(TimeSpan.FromMinutes(15), TimeParserHelper.ParseTime("+ 15 = 430"));
        Assert.Equal(TimeSpan.FromMinutes(20), TimeParserHelper.ParseTime("+ 20 = 225 🙏"));
    }
    
    [Fact]
    public void WithTextTest()
    {
        Assert.Equal(TimeSpan.FromMinutes(15), TimeParserHelper.ParseTime("+15🪬\nОбщее 295\nВчера 546"));
    }
    
    [Fact]
    public void TwoTimesTest()
    {
        Assert.Equal(TimeSpan.FromMinutes(60), TimeParserHelper.ParseTime("+30 + 30 = 205 🧘"));
        Assert.Equal(TimeSpan.FromMinutes(60), TimeParserHelper.ParseTime("+30+30=60"));
        Assert.Equal(TimeSpan.FromMinutes(30), TimeParserHelper.ParseTime("+15\n+15\nИтого 486"));
    }
    
    [Fact]
    public void TotalResultTest()
    {
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("435"));
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("150"));
    }
    
    [Fact]
    public void WithoutPlusTest()
    {
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("30=120"));
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("25🙏"));
    }
}