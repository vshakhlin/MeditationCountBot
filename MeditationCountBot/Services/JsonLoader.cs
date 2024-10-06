using System.Text.Json;
using MeditationCountBot.Dto;

namespace MeditationCountBot.Services;

public class JsonLoader : IJsonLoader
{
    public async Task<Dictionary<string, CounterDto>> LoadAllJsons()
    {
        var dictCounters = new Dictionary<string, CounterDto>();
        var files = Directory.GetFiles("../chats");
        foreach (var filePath in files)
        {
            var counterDto = await LoadFromJson(filePath);
            if (!dictCounters.ContainsKey(counterDto.ChatId))
            {
                dictCounters.Add(counterDto.ChatId, counterDto);
            }
        }

        return dictCounters;
    }

    private async Task<CounterDto> LoadFromJson(string filePath)
    {
        var json = await File.ReadAllTextAsync(filePath);
        var counterDto = JsonSerializer.Deserialize<CounterDto>(json);
        return counterDto;
    }

    public async Task SaveToJsonAsync(CounterDto counterDto, bool log = false)
    {
        var json = JsonSerializer.Serialize(counterDto, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });
        if (log)
        {
            Console.WriteLine(json);
        }

        try
        {
            var fileName = $"{counterDto.ChatId}.json";
            await File.WriteAllTextAsync($"../chats/{fileName}", json);
        }
        catch (DirectoryNotFoundException dirNotFoundException)
        {
            // Create and try again
            Console.WriteLine("DirectoryNotFoundException");
        }
        catch (UnauthorizedAccessException unauthorizedAccessException)
        {
            // Show a message to the user
            Console.WriteLine($"UnauthorizedAccessException {unauthorizedAccessException.Message}");
        }
        catch (IOException ioException)
        {
            Console.WriteLine($"IOException {ioException.Message}");
            // Show a message to the user
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Exception {exception.Message}");
            // Show general message to the user
        }
    }
}