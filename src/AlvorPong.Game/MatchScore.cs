namespace AlvorPong.Game;

/// <summary>Tracks match points and decides the winner.</summary>
[Match]
public class MatchScore
{
    public const int WinScore = 5;
    private const int MaximumPointCount = WinScore * 2 - 1;

    private readonly MatchSide[] history = new MatchSide[MaximumPointCount];
    private int pointCount;
    private int left;
    private int right;

    /// <summary>Gets the left player's score.</summary>
    public int Left => left;

    /// <summary>Gets the right player's score.</summary>
    public int Right => right;

    /// <summary>Gets the scoring side of every point in match order.</summary>
    public ReadOnlySpan<MatchSide> History => history.AsSpan(0, pointCount);

    public MatchSide? Winner =>
        left >= WinScore ? MatchSide.Left
        : right >= WinScore ? MatchSide.Right
        : null;

    public void Add(MatchSide side)
    {
        history[pointCount++] = side;
        if (side == MatchSide.Left)
            left++;
        else
            right++;
    }
}
