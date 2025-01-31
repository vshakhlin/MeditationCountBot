using System.Text;
using System.Text.Json;
using MeditationCountBot.Dto;
using MeditationCountBot.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace MeditationCountBot.Tests;

public class LogReaderTests
{
    [Fact]
    public void ReadLogfile5()
    {
        var logReader = new LogReader();
        var updates = logReader.ReadLogs("..\\..\\..\\..\\backup\\logs", "log20250131.txt");

        foreach (var update in updates)
        {
            if (update.Message != null)
            {
                if (update.Message.Date < DateTime.Parse("2025-01-30 21:00:00"))
                {
                    Console.WriteLine(update.Message.Chat.Id);
                    Console.WriteLine(update.Message.Date);
                    Console.WriteLine(update.Message.From.Id);
                    Console.WriteLine(
                        $"{update.Message.From.FirstName} {update.Message.From.LastName}: {update.Message.Text}");
                }
            }
        }
        // var logs = File.ReadAllLines("..\\..\\..\\..\\backup\\logs\\log20250130.txt");
        // var regexp = new Regex(@"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}\s\+\d{2}:\d{2}\s\[INF\]\s{");
        // var couner = 0;
        // foreach (var logLine in logs)
        // {
        //     if (regexp.IsMatch(logLine))
        //     {
        //         couner++;
        //     }
        // }
        // Console.WriteLine(couner);
    }

    [Fact]
    public void ReadLogfile3()
    {
        var totalAmounts = File.ReadAllLines("..\\..\\..\\..\\backup\\logs\\total_amount.csv");
        var sixMonthAmounts = File.ReadAllLines("..\\..\\..\\..\\backup\\logs\\six_mounth.csv");
        var addresses = File.ReadAllLines("..\\..\\..\\..\\backup\\logs\\address.csv");
        var totalAmountMap = ToMap(totalAmounts);
        var sixMonthAmountsMap = ToMap(sixMonthAmounts);
        var addressesMap = ToMap(addresses);

        var emails = new List<string>()
        {
            
        };

        var sb = new StringBuilder();
        foreach (var email in emails)
        {

            sb.Append(email).Append(',');
            sb.Append(totalAmountMap.ContainsKey(email) ? "Yes" : "No").Append(',');
            sb.Append(totalAmountMap.ContainsKey(email) ? totalAmountMap[email] : 0).Append(',');
            sb.Append(sixMonthAmountsMap.ContainsKey(email) ? sixMonthAmountsMap[email] : 0).Append(',');
            sb.Append(addressesMap.ContainsKey(email) ? addressesMap[email] : "").Append(',');
            sb.Append("\n");

        }
        
        File.WriteAllText("..\\..\\..\\..\\backup\\logs\\result.csv", sb.ToString());
    }

    private Dictionary<string, string> ToMap(string[] values)
    {
        var dict = new Dictionary<string, string>();
        foreach (var val in values)
        {
            var split = val.Split(',');
            dict.Add(split[0], split[1]);
        }

        return dict;
    }

    [Fact]
    public void ReadLogfile4()
    {
        var logs = File.ReadAllLines("..\\..\\..\\..\\backup\\logs\\log20250128.txt");
        var path = "..\\..\\..\\..\\backup";
        var flag = false;
        var sb = new StringBuilder();
        foreach (var logLine in logs)
        {
            if (logLine.Contains("ChatId"))
            {
                sb.Append("{");
                flag = true;
            }

            if (flag && logLine.StartsWith("2025"))
            {
                flag = false;
                var counter = JsonSerializer.Deserialize<CounterDto>(sb.ToString());
                Console.WriteLine("-------------");
                Console.WriteLine(counter.ChatId);
                Console.WriteLine(counter.Participants.Count);
                
                var savedDtoJson = JsonSerializer.Serialize(counter, new JsonSerializerOptions()
                {
                    WriteIndented = true,
                });

                var fileName = $"{counter.ChatId}.json";
                File.WriteAllText(Path.Combine(path, fileName), savedDtoJson);
                sb.Clear();
            }

            if (flag)
            {
                sb.Append(logLine);
            }
        }
    }

    [Fact]
    public void ReadLogfile()
    {
        var logs = File.ReadAllLines("..\\..\\..\\..\\backup\\logs\\logs2.txt");
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
                    update = JsonSerializer.Deserialize<Update>(logJsonStr);
                }
                catch
                {
                    update = JsonSerializer.Deserialize<Update>(logJsonStr + "}");
                }

                if (update != null)
                {
                    if (update.Type == UpdateType.Message)
                    {
                        if (true || update.Message.Chat.Id == -1002357221153)
                        {
                            var text = update.Message.Text;
                            if (string.IsNullOrEmpty(text))
                            {
                                text = update.Message.Caption;
                            }

                            Console.WriteLine(update.Message.Chat.Id);
                            Console.WriteLine(update.Message.Date);
                            Console.WriteLine(
                                $"{update.Message.From.FirstName} {update.Message.From.LastName}: {text}");
                        }
                    }
                    else if (update.Type == UpdateType.EditedMessage)
                    {
                        if (true || update.EditedMessage.Chat.Id == -1002357221153)
                        {
                            var text = update.EditedMessage.Text;
                            if (string.IsNullOrEmpty(text))
                            {
                                text = update.EditedMessage.Caption;
                            }

                            Console.WriteLine("-----");
                            
                            Console.WriteLine(update.EditedMessage.Chat.Id);
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
    
    [Fact]
    public void ReadLogfile2()
    {
        var logs = File.ReadAllLines("..\\..\\..\\..\\backup\\logs\\logs2.txt");
        foreach (var logLine in logs)
        {
            var logJsonStr = logLine;
            if (logJsonStr.Trim().StartsWith("{"))
            {
                Update update;
                try
                {
                    update = JsonSerializer.Deserialize<Update>(logJsonStr);
                }
                catch
                {
                    update = JsonSerializer.Deserialize<Update>(logJsonStr + "}");
                }

                if (update != null)
                {
                    if (update.Type == UpdateType.Message)
                    {
                        if (true || update.Message.Chat.Id == -1002357221153)
                        {
                            var text = update.Message.Text;
                            if (string.IsNullOrEmpty(text))
                            {
                                text = update.Message.Caption;
                            }

                            var time = TimeParserHelper.ParseTime(text);
                            if (TimeSpan.Zero != time && update.Message.Date > DateTime.Parse("2024-11-27 00:00:00") && update.Message.Date < DateTime.Parse("2024-11-27 21:00:00"))
                            {
                                Console.WriteLine(update.Message.Chat.Id);
                                Console.WriteLine(update.Message.Date);
                                Console.WriteLine(
                                    $"{update.Message.From.Id} {update.Message.From.Username}");
                                Console.WriteLine(
                                    $"{update.Message.From.FirstName} {update.Message.From.LastName}: {text}");
                            }
                        }
                    }
                    else if (update.Type == UpdateType.EditedMessage)
                    {
                        if (true || update.EditedMessage.Chat.Id == -1002357221153)
                        {
                            var text = update.EditedMessage.Text;
                            if (string.IsNullOrEmpty(text))
                            {
                                text = update.EditedMessage.Caption;
                            }

                            var time = TimeParserHelper.ParseTime(text);
                            if (TimeSpan.Zero != time && update.EditedMessage.Date > DateTime.Parse("2024-11-27 00:00:00") && update.EditedMessage.Date < DateTime.Parse("2024-11-27 21:00:00"))
                            {
                                Console.WriteLine("-----");

                                Console.WriteLine(update.EditedMessage.Chat.Id);
                                Console.WriteLine(update.EditedMessage.Date);
                                Console.WriteLine(
                                    $"{update.EditedMessage.From.Id} {update.EditedMessage.From.Username}");
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
}