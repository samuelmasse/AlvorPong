namespace AlvorPong.Game.Frontend;

/// <summary>Fonts used by the match renderer, seeded by the menu/composition layer.</summary>
[Match]
public class MatchFonts(Font font)
{
    public FontSize Score { get; } = font.Size(54);

    public FontSize Countdown { get; } = font.Size(140);
}
