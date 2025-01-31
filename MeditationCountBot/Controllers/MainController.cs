using MeditationCountBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeditationCountBot.Controllers;

[ApiController]
[Route("api/main")]
public class MainController : ControllerBase
{
    private readonly IMessagesStore _messagesStore;
    private readonly ICounterService _counterService;
    private readonly ICalculateResultService _calculateResultService;
    private readonly IReCalculateResultService _reCalculateResultService;
    private readonly IFreeTextMessageSender _messageSender;
    public MainController(
        ICounterService counterService,
        IMessagesStore messagesStore,
        ICalculateResultService calculateResultService,
        IFreeTextMessageSender messageSender,
        IReCalculateResultService reCalculateResultService)
    {
        _counterService = counterService;
        _messagesStore = messagesStore;
        _calculateResultService = calculateResultService;
        _messageSender = messageSender;
        _reCalculateResultService = reCalculateResultService;
    }

    [HttpGet("calculate", Name = "Calculate total results and send to telegram groups")]
    public async Task<ActionResult<List<string>>> CalculateTotalAndSend([FromQuery] string mode = "test")
    {
        var results = await _calculateResultService.CalculateTotalResultsAndSend();
        return Ok(results);
    }

    [HttpGet("recalculate", Name = "Re calculate count days if something wrong")]
    public async Task<ActionResult<List<string>>> ReCalculate([FromQuery] string log, [FromQuery] DateTime date)
    {
        var results = await _reCalculateResultService.ReCalculate(log, date);
        return Ok(results);
    }
    
    [HttpGet("message", Name = "Sent free text message to telegram groups")]
    public async Task<ActionResult<List<string>>> MessageSend([FromQuery] string message, [FromQuery] string mode = "test")
    {
        var results = await _messageSender.SendMessageToAllGroups(message, mode, new CancellationToken());
        return Ok(results);
    }
    
    [HttpGet("reload", Name = "Reload from files")]
    public async Task<ActionResult<bool>> Reload()
    {
        await _counterService.Reload();
        await _messagesStore.Reload();

        return Ok(true);
    }
}