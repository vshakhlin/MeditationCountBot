using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace MeditationCountBot.Tests;

public class TelegramBotClientMock : ITelegramBotClient
{
    public async Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken())
    {
        if (request.MethodName == "getChatAdministrators")
        {
            var chatAdmin = new ChatMemberAdministrator
            {
                IsAnonymous = false,
                CanBeEdited = true,
                CanChangeInfo = true,
                CanDeleteMessages = true,
                CanEditMessages = true,
                CanInviteUsers = true,
                CanManageChat = true,
                CanManageTopics = true,
                CanPinMessages = true,
                CanPostMessages = true,
                CanRestrictMembers = true,
                CanManageVideoChats = true,
                CanPromoteMembers = true,
                CustomTitle = "admin",
                User = new User
                {
                    Id = 453949424,
                    IsBot = false,
                    LastName = "akbayev",
                    FirstName = "beibit",
                    Username = "",
                    IsPremium = false,
                }
            };
            
            var result = new ChatMember[]
            {
                chatAdmin
            };
            var json = JsonConvert.SerializeObject(result);
            return await Task.FromResult(JsonConvert.DeserializeObject<TResponse>(json));
        }

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