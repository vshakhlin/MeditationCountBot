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

    public MainController(
        ICounterService counterService,
        IMessagesStore messagesStore,
        ICalculateResultService calculateResultService)
    {
        _counterService = counterService;
        _messagesStore = messagesStore;
        _calculateResultService = calculateResultService;
    }

    [HttpGet("message", Name = "Calculate total results and send to telegram groups")]
    public async Task<ActionResult<List<string>>> CalculateTotalAndSend([FromQuery] string mode = "test")
    {
        var results = await _calculateResultService.CalculateTotalResultsAndSend();
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