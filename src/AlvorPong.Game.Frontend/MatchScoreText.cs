namespace AlvorPong.Game.Frontend;

/// <summary>Caches the finite set of score strings used by the per-frame match renderer.</summary>
[Match]
public class MatchScoreText
{
    private const int ValueCount = MatchScore.WinScore + 1;

    private readonly string[] texts = new string[ValueCount * ValueCount];

    public MatchScoreText()
    {
        for (var left = 0; left < ValueCount; left++)
        {
            for (var right = 0; right < ValueCount; right++)
                texts[left * ValueCount + right] = $"{left}   {right}";
        }
    }

    /// <summary>Gets the cached text for one valid match score.</summary>
    public string Get(int left, int right) => texts[left * ValueCount + right];
}
