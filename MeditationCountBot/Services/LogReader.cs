using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace MeditationCountBot.Services;

public class LogReader : ILogReader
{
    public const string LogsPath = "../backup/logs";
    
    public List<Update> ReadLogs(string logPath, string logFile)
    {
        var result = new List<Update>();
        var logs = File.ReadAllLines(Path.Combine(logPath, logFile));
        var regexp = new Regex(@"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}\s\+\d{2}:\d{2}\s\[INF\]\s{");
        foreach (var logLine in logs)
        {
            if (regexp.IsMatch(logLine))
            {
                var logLineReplace = regexp.Replace(logLine, "{");
                Update update;
                try
                {
                    update = JsonSerializer.Deserialize<Update>(logLineReplace);
                }
                catch
                {
                    try
                    {
                        update = JsonSerializer.Deserialize<Update>(logLineReplace + "}");
                    }
                    catch
                    {
                        update = null;
                    }
                }

                if (update != null)
                {
                    result.Add(update);
                }
            }
        }
        
        return result;
    }
}