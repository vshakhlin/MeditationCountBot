using System.Text.RegularExpressions;

namespace MeditationCountBot.Services;

public class MarkdownHelper
{
    public static string Escape(string input)
    {
        var pattern = @"[_*[\]()~`>#\+\-=|{}.!]";
        var result = Regex.Replace(input, pattern, m =>
            m.Groups[1].Success ? m.Groups[1].Value : $@"\{m.Value}");
        return result;
    }
}