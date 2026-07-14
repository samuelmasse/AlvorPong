namespace AlvorPong.Game;

/// <summary>Owns nondeterministic choices for one match.</summary>
[Match]
public class MatchRandom
{
    private readonly Random random = new();

    /// <summary>Returns a uniformly distributed value from -1 inclusive to 1 exclusive.</summary>
    public float NextSignedSingle() => random.NextSingle() * 2f - 1f;

    /// <summary>Returns a uniformly distributed value from 0 inclusive to 1 exclusive.</summary>
    public double NextDouble() => random.NextDouble();

    /// <summary>Returns a uniformly selected side.</summary>
    public MatchSide NextSide() => random.Next(2) == 0 ? MatchSide.Left : MatchSide.Right;
}
