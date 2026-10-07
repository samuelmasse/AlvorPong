namespace AlvorPong;

/// <summary>Registers maintained Ent bags and publishes the initialized match Ents.</summary>
[MatchLoader]
public class MatchEntLoader(
    MatchEntIdxContext context,
    MatchPaddleBag paddles,
    MatchBallBag balls,
    MatchEntLifetime ents)
{
    /// <summary>Completes Indexed registration before the first Ent allocation.</summary>
    public void Run()
    {
        context.AddGatedBag(paddles);
        context.AddGatedBag(balls);
        ents.Load();
    }
}
