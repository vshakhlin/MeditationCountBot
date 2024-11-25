using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace MeditationCountBot.Tests;

public class LogReader
{
    [Fact]
    public void ReadLogfile()
    {
        var logs = File.ReadAllLines("..\\..\\..\\..\\backup\\logs\\log20241108.txt");
        foreach (var logLine in logs)
        {
            if (logLine.Length < 39)
            {
                continue;
            }

            var logJsonStr = logLine.Substring(37) + "}";
            if (logJsonStr.StartsWith("{"))
            {
                Update update;
                try
                {
                    update = JsonConvert.DeserializeObject<Update>(logJsonStr);
                }
                catch
                {
                    update = JsonConvert.DeserializeObject<Update>(logJsonStr + "}");
                }

                if (update != null)
                {
                    if (update.Type == UpdateType.Message)
                    {
                        if (update.Message.Chat.Id == -1002357221153)
                        {
                            var text = update.Message.Text;
                            if (string.IsNullOrEmpty(text))
                            {
                                text = update.Message.Caption;
                            }

                            Console.WriteLine(update.Message.Date);
                            Console.WriteLine(
                                $"{update.Message.From.FirstName} {update.Message.From.LastName}: {text}");
                        }
                    }
                    else if (update.Type == UpdateType.EditedMessage)
                    {
                        if (update.EditedMessage.Chat.Id == -1002357221153)
                        {
                            var text = update.EditedMessage.Text;
                            if (string.IsNullOrEmpty(text))
                            {
                                text = update.EditedMessage.Caption;
                            }

                            Console.WriteLine("-----");
                            Console.WriteLine(update.EditedMessage.Date);
                            Console.WriteLine(
                                $"{update.EditedMessage.From.FirstName} {update.EditedMessage.From.LastName}: {text}");
                            Console.WriteLine("-----");
                        }
                    }
                }
            }
        }
    }
}