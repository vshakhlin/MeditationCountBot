using MeditationCountBot.Dto;
using MeditationCountBot.Services;
using MeditationCountBot.Telegram;
using Microsoft.AspNetCore.Mvc;

namespace MeditationCountBot.Controllers;

[ApiController]
[Route("api/main")]
public class MainController : ControllerBase
{
    private readonly IJsonLoader _jsonLoader;
    private readonly IJsonLogger _jsonLogger;
    private readonly ITelegramMessageSender _messageSender;
    private readonly IMessageFormer _messageFormer;
    private readonly ICounterService _counterService;
    private readonly ICalculateContinuouslyService _calculateContinuouslyService;

    public MainController(IMessageFormer messageFormer, IJsonLoader jsonLoader, ITelegramMessageSender messageSender, ICounterService counterService, ICalculateContinuouslyService calculateContinuouslyService, IJsonLogger jsonLogger)
    {
        _messageFormer = messageFormer;
        _jsonLoader = jsonLoader;
        _messageSender = messageSender;
        _counterService = counterService;
        _calculateContinuouslyService = calculateContinuouslyService;
        _jsonLogger = jsonLogger;
    }

    [HttpGet("message", Name = "Get message")]
    public async Task<ActionResult<string>> GetMessage([FromQuery] string mode = "test")
    {
        
        var now = DateTime.UtcNow.AddHours(3);
        Console.WriteLine($"Start calculation {now}");

        var result = "";
        var dict = await _jsonLoader.LoadAllJsons();
        Console.WriteLine($"Count: {dict.Count}");
        foreach (var counter in dict.Values)
        {
            if (mode == "test" && counter.ChatId != "115601429")
            {
                continue;
            }

            _calculateContinuouslyService.CalculateContinuouslyDays(counter, DateTime.UtcNow);
            var message = _messageFormer.CreateMessage(counter);
            Console.WriteLine(message);
            
            counter.Yesterday = counter.Today;
            if (counter.Best < counter.Today)
            {
                counter.Best = counter.Today;
            }
            
            counter.Today = TimeSpan.Zero;
            await _jsonLoader.SaveToJsonAsync(counter, true);
            Console.WriteLine($"SAVED");
                
            var sendResult = await _messageSender.SendMessage(counter.ChatId, message, "ru", null, new CancellationToken());
            Console.WriteLine($"SEND: {sendResult.IsSuccess}");
            var sendResultMessage = "";
            if (!string.IsNullOrEmpty(sendResult.ErrorMessage))
            {
                sendResultMessage = $"Error: {sendResult.ErrorMessage}";
            }
            else
            {
                sendResultMessage = $"Send {counter.ChatId} at {DateTime.UtcNow.AddHours(3)}";
            }
            
            Console.WriteLine(sendResultMessage);
            result += sendResultMessage;
        }
        
        await _counterService.Reload();

        return Ok(result);
    }
    
    [HttpGet("recalculate", Name = "Recalculate")]
    public async Task<ActionResult<bool>> Recalculate()
    {
        var dict = await _jsonLoader.LoadAllJsons();
        foreach (var counter in dict.Values)
        {
            _calculateContinuouslyService.CalculateContinuouslyDays(counter, DateTime.UtcNow.AddDays(-1));
            var message = _messageFormer.CreateMessage(counter);
            await _messageSender.SendMessage("115601429", message, "ru", null, new CancellationToken());

            counter.Yesterday = counter.Today;
            if (counter.Best < counter.Today)
            {
                counter.Best = counter.Today;
            }

            counter.Today = TimeSpan.Zero;
            await _jsonLoader.SaveToJsonAsync(counter);
        }

        return Ok(true);
    }
    
    [HttpGet("reload", Name = "Reload from files")]
    public async Task<ActionResult<bool>> Reload()
    {
        await _counterService.Reload();

        return Ok(true);
    }
    
    [HttpGet("log")]
    public async Task<ActionResult<bool>> Log()
    {
        await _jsonLogger.Log("115601429", new JsonLog()
        {
            MessageDate = DateTime.Now,
            Text = "+25",
            Time = TimeSpan.FromMinutes(25),
            TotalTime = TimeSpan.FromMinutes(130),
            UserId = 123256
        });

        return Ok(true);
    }
}