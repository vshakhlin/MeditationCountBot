using MeditationCountBot.Dto;

namespace MeditationCountBot.Services;

public interface IMessagesStore
{
    Task Initialize();
    Task Reload();
    Task Clear(string chatId);
    MessageLog? Load(string chatId, int messageId);
    Task Save(string chatId, MessageLog messageLog);
    Task ReSave(string chatId, MessageLog messageLog);
}