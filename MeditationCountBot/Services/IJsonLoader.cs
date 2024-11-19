namespace MeditationCountBot.Services;

public interface IJsonLoader
{
    Task<Dictionary<string, T>> LoadAllJsons<T>(string path);
    Task SaveToJsonAsync<T>(string chatId, T counterDto, string path, bool log = false);
}