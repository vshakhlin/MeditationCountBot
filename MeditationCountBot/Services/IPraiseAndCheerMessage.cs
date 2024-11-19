namespace MeditationCountBot.Services;

public interface IPraiseAndCheerMessage
{
    string GetRandomPraiseMessage();

    string GetRandomCheerMessage();
}