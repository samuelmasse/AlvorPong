namespace AlvorPong.Menus;

/// <summary>Application style: the Blend design system plus AlvorPong's game-local recipes and colors.</summary>
[App]
public class AppStyle(RootInter inter, RootGl gl, RootUiScale scale, RootKeyboard keyboard)
    : BlendStyle(inter, gl, scale, keyboard)
{
    private const float KeyChipHeight = 16f;
    private const float KeyChipTextPadding = 5f;
    private const int KeyChipFontSize = 10;

    /// <summary>Gets the field foreground at the decorative banner alpha.</summary>
    public Vec4 FieldFaint { get; } = (0.92f, 0.94f, 0.96f, 0.25f);

    /// <summary>Gets the point-tape color for points won by the left player.</summary>
    public Vec4 TapeLeftPoint { get; } = (0.92f, 0.94f, 0.96f, 0.9f);

    /// <summary>Gets the point-tape color for points won by the right player.</summary>
    public Vec4 TapeRightPoint => Palette.WithAlpha(Palette.MutedText, 0.4f);

    /// <summary>Applies a small display-only key cap, sized from its text.</summary>
    public void KeyChip(EntMut ent) => ent.Mutate()
        .Mutate(Board)
        .SizeRelativeV((0, 0))
        .SizeTextRelativeV((1, 0))
        .SizeV((0, KeyChipHeight))
        .FontV(TextFont)
        .FontSizeV(KeyChipFontSize)
        .TextPaddingV((KeyChipTextPadding, 0, KeyChipTextPadding, 0))
        .TextAlignmentV(Alignment.Center)
        .TextColorV(Palette.MutedText)
        .ColorV(Palette.Raised)
        .Mutate(StrongBorder);

    /// <summary>Adds a one-pixel accent border around a node.</summary>
    public void AccentBorder(EntMut ent)
    {
        Rule(ent, Alignment.Top | Alignment.Left, (1, 0), (0, Metrics.Hairline), Palette.Accent);
        Rule(ent, Alignment.Bottom | Alignment.Left, (1, 0), (0, Metrics.Hairline), Palette.Accent);
        Rule(ent, Alignment.Top | Alignment.Left, (0, 1), (Metrics.Hairline, 0), Palette.Accent);
        Rule(ent, Alignment.Top | Alignment.Right, (0, 1), (Metrics.Hairline, 0), Palette.Accent);
    }
}
