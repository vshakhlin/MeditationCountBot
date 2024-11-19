using MeditationCountBot.Dto;

namespace MeditationCountBot.Services;

public class MessagesStore : IMessagesStore
{
    private readonly IJsonLoader _jsonLoader;
    private Dictionary<string, List<MessageLog>> _dictMessages;
    
    public MessagesStore(IJsonLoader jsonLoader)
    {
        _jsonLoader = jsonLoader;
    }

    public async Task Initialize()
    {
        await Reload();
    }

    public async Task Reload()
    {
        _dictMessages = await _jsonLoader.LoadAllJsons<List<MessageLog>>(JsonLoader.MessageLogPath);
    }
    
    public MessageLog? Load(string chatId, int messageId)
    {
        var messageStore = GetOrCreateMessageStore(chatId);
        return messageStore.FirstOrDefault(m => m.MessageId == messageId);
    }
    
    public async Task Clear(string chatId)
    {
        var messageStore = new List<MessageLog>();
        await _jsonLoader.SaveToJsonAsync(chatId, messageStore, JsonLoader.MessageLogPath);
    }
    
    public async Task Save(string chatId, MessageLog messageLog)
    {
        var messageStore = GetOrCreateMessageStore(chatId);
        messageStore.Add(messageLog);
        await _jsonLoader.SaveToJsonAsync(chatId, messageStore, JsonLoader.MessageLogPath);
    }
    
    public async Task ReSave(string chatId, MessageLog messageLog)
    {
        var messageStore = GetOrCreateMessageStore(chatId);
        var editingIndex = messageStore.FindIndex(m => m.MessageId == messageLog.MessageId);
        messageStore[editingIndex] = messageLog;
        await _jsonLoader.SaveToJsonAsync(chatId, messageStore, JsonLoader.MessageLogPath);
    }
    
    private List<MessageLog> GetOrCreateMessageStore(string chatId)
    {
        List<MessageLog> messageStore;
        if (_dictMessages.ContainsKey(chatId))
        {
            messageStore = _dictMessages[chatId];
        }
        else
        {
            messageStore = new List<MessageLog>();
            _dictMessages.Add(chatId, messageStore);
        }
        return messageStore;
    }
}