using System.Text.RegularExpressions;

namespace MeditationCountBot.Services;

public class TimeParserHelper
{
    const string UrlPattern = @"https?";
    const string BasePattern = @"\+\s?\d+";
    const string OnlyDigitalPattern = @"\d+";

    public static TimeSpan ParseTime(string text, TimeSpan total)
    {
        if (string.IsNullOrEmpty(text))
        {
            return TimeSpan.Zero;
        }
        
        if (Regex.IsMatch(text, UrlPattern))
        {
            return TimeSpan.Zero;
        }
        
        var baseTime = BaseParse(text);
        if (baseTime != TimeSpan.Zero)
        {
            if (baseTime > TimeSpan.FromHours(9))
            {
                return TimeSpan.Zero;
            }
            return baseTime;
        }
        
        // var onlyDigitalTime = OnlyDigitalParse(text, total);
        // if (onlyDigitalTime != TimeSpan.Zero)
        // {
        //     return onlyDigitalTime;
        // }
            
        return TimeSpan.Zero;
    }

    private static TimeSpan BaseParse(string text)
    {
        var baseTime = TimeSpan.Zero;
        foreach (Match match in Regex.Matches(text, BasePattern, RegexOptions.IgnoreCase))
        {
            var minutes = match.Value.Replace("+", "");
            var parseMinutes = TimeSpan.FromMinutes(int.Parse(minutes));
            baseTime += parseMinutes;
        }

        return baseTime;
    }
    
    private static TimeSpan OnlyDigitalParse(string text, TimeSpan total)
    {
        var baseTime = TimeSpan.Zero;
        var wordCount = WordCount(text);
        if (wordCount < 6)
        {
            foreach (Match match in Regex.Matches(text, OnlyDigitalPattern, RegexOptions.IgnoreCase))
            {
                Console.WriteLine("{0} (duplicates '{1}') at position {2}", match.Value, match.Groups[1].Value,
                    match.Index);
                var minutes = match.Value;
                var parseMinutes = TimeSpan.FromMinutes(int.Parse(minutes));
                if (parseMinutes > TimeSpan.FromMinutes(5) && parseMinutes <= TimeSpan.FromMinutes(60))
                {
                    return parseMinutes;
                }

                Console.WriteLine($"parser minutes {parseMinutes} total {total}");
                if (parseMinutes > TimeSpan.FromMinutes(60) && parseMinutes > total)
                {
                    return parseMinutes - total;
                }
            }
        }

        return baseTime;
    }

    private static int WordCount(string text)
    {
        return text.Split(" ").Length;
    }
}