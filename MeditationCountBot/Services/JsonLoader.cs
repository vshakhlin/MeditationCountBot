using System.Text.Json;

namespace MeditationCountBot.Services;

public class JsonLoader(ILogger<JsonLoader> logger) : IJsonLoader
{
    public const string BackupPath = "../backup";
    public const string ChatsPath = "../chats";
    public const string MessageLogPath = "../messages";

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
    
    public async Task Backup<T>(string chatId, T counter)
    {
        await SaveToJsonAsync(chatId, counter, BackupPath, false);
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
            logger.LogInformation(savedDtoJson);
        }

        try
        {
            var fileName = $"{chatId}.json";
            await File.WriteAllTextAsync($"{path}/{fileName}", savedDtoJson);
        }
        catch (DirectoryNotFoundException dirNotFoundException)
        {
            // Create and try again
            logger.LogError(dirNotFoundException, ("DirectoryNotFoundException"));
        }
        catch (UnauthorizedAccessException unauthorizedAccessException)
        {
            // Show a message to the user
            logger.LogError(unauthorizedAccessException, $"UnauthorizedAccessException {unauthorizedAccessException.Message}");
        }
        catch (IOException ioException)
        {
            logger.LogError(ioException, $"IOException {ioException.Message}");
            // Show a message to the user
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"Exception {exception.Message}");
            // Show general message to the user
        }
    }
}