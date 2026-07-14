namespace AlvorPong.Game.Frontend;

/// <summary>Fonts used by the match renderer, seeded by the menu/composition layer.</summary>
[Match]
public class MatchFonts(Font font)
{
    private readonly FontSize score = font.Size(54);
    private readonly FontSize countdown = font.Size(140);

    /// <summary>Gets the font used for the running score.</summary>
    public FontSize Score => score;

    /// <summary>Gets the font used for the serve countdown.</summary>
    public FontSize Countdown => countdown;
}
