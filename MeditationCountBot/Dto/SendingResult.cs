namespace MeditationCountBot.Dto;

public class SendingResult
{
    public bool IsSuccess { get; set; }
        
    public object? Data { get; set; }

    public string? ErrorMessage { get; set; }
}