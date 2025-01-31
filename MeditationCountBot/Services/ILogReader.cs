using Telegram.Bot.Types;

namespace MeditationCountBot.Services;

public interface ILogReader
{
    List<Update> ReadLogs(string logPath, string logFile);
}