namespace AlvorPong.Game;

/// <summary>Registers maintained Ent bags and publishes the initialized match Ents.</summary>
[MatchLoader]
public sealed class MatchEntLoader(
    MatchEntIdxContextBuilder context,
    MatchPaddleBagMut paddles,
    MatchBallBagMut balls,
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
