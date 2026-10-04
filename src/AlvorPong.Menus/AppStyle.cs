namespace AlvorPong;

/// <summary>Application style: the Blend design system plus AlvorPong's game-local recipes and colors.</summary>
[App]
public class AppStyle(BlendUi bl)
{
    private const float KeyChipHeight = 16f;
    private const float KeyChipTextPadding = 5f;
    private const int KeyChipFontSize = 10;

    private readonly Vec4 fieldFaint = (0.92f, 0.94f, 0.96f, 0.25f);

    /// <summary>Gets the field foreground at the decorative banner alpha.</summary>
    public Vec4 FieldFaint => fieldFaint;

    private readonly Vec4 tapeLeftPoint = (0.92f, 0.94f, 0.96f, 0.9f);

    /// <summary>Gets the point-tape color for points won by the left player.</summary>
    public Vec4 TapeLeftPoint => tapeLeftPoint;

    /// <summary>Gets the point-tape color for points won by the right player.</summary>
    public Vec4 TapeRightPoint => bl.S.Palette.WithAlpha(bl.S.Palette.MutedText, 0.4f);

    /// <summary>Applies a small display-only key cap, sized from its text.</summary>
    public void KeyChip(EntMut ent) => ent.Mutate()
        .Mutate(bl.S.Board)
        .SizeRelativeV((0, 0))
        .SizeTextRelativeV((1, 0))
        .SizeV((0, KeyChipHeight))
        .FontV(bl.S.TextFont)
        .FontSizeV(KeyChipFontSize)
        .TextPaddingV((KeyChipTextPadding, 0, KeyChipTextPadding, 0))
        .TextAlignmentV(Alignment.Center)
        .TextColorV(bl.S.Palette.MutedText)
        .ColorV(bl.S.Palette.Raised)
        .Mutate(bl.S.StrongBorder);

    /// <summary>Adds a one-pixel accent border around a node.</summary>
    public void AccentBorder(EntMut ent)
    {
        BlendStyle.Rule(ent, Alignment.Top | Alignment.Left, (1, 0), (0, bl.S.Metrics.Hairline), bl.S.Palette.Accent);
        BlendStyle.Rule(ent, Alignment.Bottom | Alignment.Left, (1, 0), (0, bl.S.Metrics.Hairline), bl.S.Palette.Accent);
        BlendStyle.Rule(ent, Alignment.Top | Alignment.Left, (0, 1), (bl.S.Metrics.Hairline, 0), bl.S.Palette.Accent);
        BlendStyle.Rule(ent, Alignment.Top | Alignment.Right, (0, 1), (bl.S.Metrics.Hairline, 0), bl.S.Palette.Accent);
    }
}
