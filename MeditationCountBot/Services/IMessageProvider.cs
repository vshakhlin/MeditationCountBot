namespace MeditationCountBot.Services;

public interface IMessageProvider
{
    string TogetherTimeMessage { get; }
    string ContinueMessage { get; }
    string InstructionMessage { get; }
}