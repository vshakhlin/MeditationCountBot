using System.Text.Json;

namespace MeditationCountBot.Services;

public class JsonLoader : IJsonLoader
{
    public const string ChatsPath = "../chats";
    public const string MessageLogPath = "../messages";

    private readonly ILogger<JsonLoader> _logger;

    public JsonLoader(ILogger<JsonLoader> logger)
    {
        _logger = logger;
    }

    public async Task<Dictionary<string, T>> LoadAllJsons<T>(string path)
    {
        var dictObjects = new Dictionary<string, T>();
        var files = Directory.GetFiles(path);
        foreach (var filePath in files)
        {
            var chatId = Path.GetFileNameWithoutExtension(filePath);
            var deserializedObj = await LoadFromJson<T>(filePath);
            if (!dictObjects.ContainsKey(chatId))
            {
                dictObjects.Add(chatId, deserializedObj);
            }
        }

        return dictObjects;
    }

    private async Task<T> LoadFromJson<T>(string filePath)
    {
        var json = await File.ReadAllTextAsync(filePath);
        var deserializeObj = JsonSerializer.Deserialize<T>(json);
        return deserializeObj;
    }

    public async Task SaveToJsonAsync<T>(string chatId, T savedDto, string path, bool log = false)
    {
        var savedDtoJson = JsonSerializer.Serialize(savedDto, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });
        if (log)
        {
            _logger.LogInformation(savedDtoJson);
        }

        try
        {
            var fileName = $"{chatId}.json";
            await File.WriteAllTextAsync($"{path}/{fileName}", savedDtoJson);
        }
        catch (DirectoryNotFoundException dirNotFoundException)
        {
            // Create and try again
            _logger.LogError(dirNotFoundException, ("DirectoryNotFoundException"));
        }
        catch (UnauthorizedAccessException unauthorizedAccessException)
        {
            // Show a message to the user
            _logger.LogError(unauthorizedAccessException, $"UnauthorizedAccessException {unauthorizedAccessException.Message}");
        }
        catch (IOException ioException)
        {
            _logger.LogError(ioException, $"IOException {ioException.Message}");
            // Show a message to the user
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Exception {exception.Message}");
            // Show general message to the user
        }
    }
}