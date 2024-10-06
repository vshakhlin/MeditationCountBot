using MeditationCountBot.Services;

namespace MeditationCountBot.Tests;

public class TimeParserHelperTest
{
    [Fact]
    public void UrlTest()
    {
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("https://mechtyumorya.ru/?ysclid=lzdzebsuue+326785678", TimeSpan.Zero));
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("mechtyumorya.ru/?ysclid=lzdzebsuue+326785678", TimeSpan.Zero));
    }
    
    [Fact]
    public void NoTimeTest()
    {
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("В Алмате тоже есть горы)", TimeSpan.Zero));
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("Экспедиция на Шри Ланку для своих с 5 по 13 декабря", TimeSpan.Zero));
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("Ребята! Поднажмем!! Уже до 500 не доползаем!!", TimeSpan.Zero));
    }
    
    [Fact]
    public void StandartTest()
    {
        Assert.Equal(TimeSpan.FromMinutes(20), TimeParserHelper.ParseTime("+20 = 415", TimeSpan.Zero));
        Assert.Equal(TimeSpan.FromMinutes(30), TimeParserHelper.ParseTime("+30= 380", TimeSpan.Zero));
        Assert.Equal(TimeSpan.FromMinutes(15), TimeParserHelper.ParseTime("+15\n501", TimeSpan.Zero));
    }
    
    [Fact]
    public void ShortTest()
    {
        Assert.Equal(TimeSpan.FromMinutes(25), TimeParserHelper.ParseTime("+25🙏", TimeSpan.Zero));
        Assert.Equal(TimeSpan.FromMinutes(15), TimeParserHelper.ParseTime("+15", TimeSpan.Zero));
        Assert.Equal(TimeSpan.FromMinutes(25), TimeParserHelper.ParseTime("+25🙏", TimeSpan.Zero));
    }
    
    [Fact]
    public void WithSpaceTest()
    {
        Assert.Equal(TimeSpan.FromMinutes(15), TimeParserHelper.ParseTime("+ 15 = 430", TimeSpan.Zero));
        Assert.Equal(TimeSpan.FromMinutes(20), TimeParserHelper.ParseTime("+ 20 = 225 🙏", TimeSpan.Zero));
    }
    
    [Fact]
    public void WithTextTest()
    {
        Assert.Equal(TimeSpan.FromMinutes(15), TimeParserHelper.ParseTime("+15🪬\nОбщее 295\nВчера 546", TimeSpan.Zero));
    }
    
    [Fact]
    public void TwoTimesTest()
    {
        Assert.Equal(TimeSpan.FromMinutes(60), TimeParserHelper.ParseTime("+30 + 30 = 205 🧘", TimeSpan.Zero));
        Assert.Equal(TimeSpan.FromMinutes(60), TimeParserHelper.ParseTime("+30+30=60", TimeSpan.Zero));
        Assert.Equal(TimeSpan.FromMinutes(30), TimeParserHelper.ParseTime("+15\n+15\nИтого 486", TimeSpan.Zero));
    }
    
    [Fact]
    public void TotalResultTest()
    {
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("435", TimeSpan.FromMinutes(405)));
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("150", TimeSpan.FromMinutes(130)));
    }
    
    [Fact]
    public void WithoutPlusTest()
    {
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("30=120", TimeSpan.Zero));
        Assert.Equal(TimeSpan.Zero, TimeParserHelper.ParseTime("25🙏", TimeSpan.Zero));
    }
}