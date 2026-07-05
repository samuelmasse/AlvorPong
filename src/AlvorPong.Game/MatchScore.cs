namespace AlvorPong.Game;

/// <summary>Tracks match points and decides the winner.</summary>
[Match]
public class MatchScore
{
    public const int WinScore = 5;

    private readonly List<MatchSide> history = [];

    public int Left { get; private set; }
    public int Right { get; private set; }

    /// <summary>Gets the scoring side of every point in match order.</summary>
    public IReadOnlyList<MatchSide> History => history;

    public MatchSide? Winner =>
        Left >= WinScore ? MatchSide.Left
        : Right >= WinScore ? MatchSide.Right
        : null;

    public void Add(MatchSide side)
    {
        history.Add(side);
        if (side == MatchSide.Left)
            Left++;
        else
            Right++;
    }
}
