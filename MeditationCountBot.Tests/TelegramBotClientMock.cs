using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace MeditationCountBot.Tests;

public class TelegramBotClientMock : ITelegramBotClient
{
    public async Task<ChatMember[]> GetChatAdministratorsAsync(
        ChatId chatId,
        CancellationToken cancellationToken = default
    )
    {
        return new ChatMember[]
        {
            new ChatMemberAdministrator()
            {
                User =
                {
                    Id = 453949424,
                    IsBot = false,
                    LastName = "akbayev",
                    FirstName = "beibit",
                    Username = null,
                }
            }
        };
    }
    
    public Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<bool> TestApiAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task DownloadFileAsync(string filePath, Stream destination,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public bool LocalBotServer { get; }
    public long? BotId { get; }
    public TimeSpan Timeout { get; set; }
    public IExceptionParser ExceptionsParser { get; set; }
    public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest;
    public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived;
}