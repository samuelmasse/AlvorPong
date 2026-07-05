namespace AlvorPong.Menus;

/// <summary>Builds the title screen: a splash card with match actions and a controls card over a status bar.</summary>
[App]
public class AppMainMenu(
    RootMetrics metrics,
    RootScreen screen,
    RootText text,
    AppMatchStart matchStart,
    AppMenuButton button,
    AppStyle s)
{
    private const float SplashWidth = 560f;
    private const float BannerHeight = 120f;
    private const float FooterHeight = 30f;
    private const float FooterTextPadding = 14f;
    private const float BodyPadding = 16f;
    private const float ActionColumnWidth = 250f;
    private const float ControlsPaddingX = 12f;
    private const float ControlsPaddingY = 10f;
    private const float ControlsCaptionHeight = 21f;
    private const float ControlRowHeight = 24f;
    private const float ControlValueColumn = 104f;
    private const float BrandmarkSize = 16f;
    private const float WordSpacing = 12f;
    private const float WordLineSpacing = 7f;
    private const int WordmarkFontSize = 34;
    private const int CaptionFontSize = 10;

    public void Create(EntMut root)
    {
        Node(root, out var panel)
            .Mutate(s.Root);
        {
            Spacer(panel);
            Splash(panel);
            Spacer(panel);
            StatusBar(panel);
        }
    }

    private static void Spacer(EntMut parent) =>
        Node(parent)
            .ColorV(default);

    private void Splash(EntMut parent)
    {
        Node(parent, out var splash)
            .AlignmentV(Alignment.Horizontal)
            .SizeWeightTypeV(SizeWeightType.Self)
            .SizeRelativeV((0, 0))
            .SizeInnerSumRelativeV((0, 1))
            .SizeV((SplashWidth, 0))
            .InnerLayoutV(InnerLayout.VerticalList)
            .ColorV(s.Palette.Panel)
            .Mutate(s.StrongBorder);
        {
            Banner(splash);
            Body(splash);
            Footer(splash);
        }
    }

    private void Banner(EntMut parent)
    {
        const float dashWidth = 4f;
        const float dashHeight = 12f;
        const float dashStep = 24f;
        const float dashStartY = 6f;
        const int dashCount = 5;
        const float paddleWidth = 10f;
        const float paddleHeight = 64f;
        const float paddleInset = 22f;
        const float ballSize = 10f;

        Node(parent, out var banner)
            .Mutate(s.Board)
            .SizeRelativeV((1, 0))
            .SizeV((0, BannerHeight))
            .ColorV(s.Palette.AppBackground)
            .Mutate(s.BottomRule);
        {
            for (var i = 0; i < dashCount; i++)
                FieldRect(banner, ((SplashWidth - dashWidth) / 2, dashStartY + i * dashStep), (dashWidth, dashHeight));

            FieldRect(banner, (paddleInset, 30), (paddleWidth, paddleHeight));
            FieldRect(banner, (SplashWidth - paddleInset - paddleWidth, 44), (paddleWidth, paddleHeight));
            FieldRect(banner, (462, 60), (ballSize, ballSize));

            Node(banner, out var word)
                .Mutate(s.VerticalList)
                .AlignmentV(Alignment.Horizontal | Alignment.Vertical)
                .InnerSpacingV(WordLineSpacing);
            {
                Node(word, out var wordRow)
                    .Mutate(s.HorizontalList)
                    .AlignmentV(Alignment.Horizontal)
                    .InnerSpacingV(WordSpacing);
                {
                    Node(wordRow)
                        .AlignmentV(Alignment.Vertical)
                        .SizeRelativeV((0, 0))
                        .SizeV((BrandmarkSize, BrandmarkSize))
                        .ColorV(s.Palette.ActiveSurface)
                        .Mutate(s.AccentBorder);

                    Node(wordRow)
                        .Mutate(s.EmphasisLabel)
                        .AlignmentV(Alignment.Vertical)
                        .FontSizeV(WordmarkFontSize)
                        .TextV("ALVORPONG");
                }

                Node(word)
                    .Mutate(s.MutedLabel)
                    .AlignmentV(Alignment.Horizontal)
                    .TextV("first to five — Pong on the AlvorKit engine");
            }
        }

        void FieldRect(EntMut banner, Vec2 offset, Vec2 size) =>
            Node(banner)
                .OffsetV(offset)
                .SizeRelativeV((0, 0))
                .SizeV(size)
                .ColorV(s.FieldFaint);
    }

    private void Body(EntMut parent)
    {
        Node(parent, out var body)
            .SizeRelativeV((1, 0))
            .SizeV((0, ActionColumnHeight() + BodyPadding * 2))
            .InnerLayoutV(InnerLayout.HorizontalList)
            .InnerSpacingV(BodyPadding)
            .PaddingV((BodyPadding, BodyPadding, BodyPadding, BodyPadding));
        {
            Actions(body);
            Controls(body);
        }
    }

    private float ActionColumnHeight() =>
        4 * s.Metrics.ButtonHeight + 4 * s.Metrics.LooseSpacing + s.Metrics.Hairline;

    private void Actions(EntMut parent)
    {
        Node(parent, out var actions)
            .Mutate(s.VerticalList)
            .SizeInnerMaxRelativeV((0, 0))
            .SizeV((ActionColumnWidth, 0))
            .InnerSpacingV(s.Metrics.LooseSpacing);
        {
            button.Create(actions, "Play vs AI", "Enter", true, () => matchStart.Run(new MatchConfig(RightIsAi: true)));
            button.Create(actions, "Play two players", "T", false, () => matchStart.Run(new MatchConfig(RightIsAi: false)));

            Node(actions)
                .SizeRelativeV((1, 0))
                .SizeV((0, s.Metrics.Hairline))
                .ColorV(s.Palette.Border);

            button.Create(actions, "Toggle fullscreen", null, false, screen.ToggleFullscreen);
            button.Create(actions, "Quit", "Esc", false, screen.Close);
        }
    }

    private void Controls(EntMut parent)
    {
        var width = SplashWidth - BodyPadding * 3 - ActionColumnWidth;
        Node(parent, out var card)
            .SizeRelativeV((0, 0))
            .SizeV((width, ActionColumnHeight()))
            .InnerLayoutV(InnerLayout.VerticalList)
            .PaddingV((ControlsPaddingX, ControlsPaddingY, ControlsPaddingX, ControlsPaddingY))
            .ColorV(s.Palette.AppBackground)
            .Mutate(s.Border);
        {
            Node(card, out var caption)
                .SizeRelativeV((1, 0))
                .SizeV((0, ControlsCaptionHeight))
                .InnerLayoutV(InnerLayout.HorizontalList)
                .InnerSizingV(InnerSizing.HorizontalWeight);
            {
                CaptionLabel(caption, "CONTROLS");
                Node(caption);
                CaptionLabel(caption, "right is AI in solo");
            }

            KeyRow(card, "left paddle", ["W", "S"]);
            KeyRow(card, "right paddle", ["I", "K"]);
            KeyRow(card, "pause", ["Esc"]);
            TextRow(card, "win", "first to 5 points");
        }

        void CaptionLabel(EntMut caption, string value) =>
            Node(caption)
                .Mutate(s.MutedLabel)
                .AlignmentV(Alignment.Vertical)
                .SizeWeightTypeV(SizeWeightType.Self)
                .FontSizeV(CaptionFontSize)
                .TextV(value);
    }

    private EntMut ControlRow(EntMut parent, string label)
    {
        Node(parent, out var row)
            .Mutate(s.MetricRow)
            .SizeV((0, ControlRowHeight));
        {
            Node(row)
                .Mutate(s.MutedLabel)
                .AlignmentV(Alignment.Left | Alignment.Vertical)
                .TextV(label);
        }

        return row;
    }

    private void KeyRow(EntMut parent, string label, string[] keys)
    {
        var row = ControlRow(parent, label);
        Node(row, out var value)
            .Mutate(s.HorizontalList)
            .AlignmentV(Alignment.Left | Alignment.Vertical)
            .OffsetV((ControlValueColumn, 0))
            .InnerSpacingV(s.Metrics.CompactSpacing);
        {
            for (var i = 0; i < keys.Length; i++)
            {
                if (i > 0)
                    Node(value)
                        .Mutate(s.MutedLabel)
                        .AlignmentV(Alignment.Vertical)
                        .TextV("/");

                Node(value)
                    .Mutate(s.KeyChip)
                    .AlignmentV(Alignment.Vertical)
                    .TextV(keys[i]);
            }
        }
    }

    private void TextRow(EntMut parent, string label, string value)
    {
        var row = ControlRow(parent, label);
        Node(row)
            .Mutate(s.MutedLabel)
            .AlignmentV(Alignment.Left | Alignment.Vertical)
            .OffsetV((ControlValueColumn, 0))
            .TextV(value);
    }

    private void Footer(EntMut parent)
    {
        Node(parent, out var footer)
            .SizeRelativeV((1, 0))
            .SizeV((0, FooterHeight))
            .InnerLayoutV(InnerLayout.HorizontalList)
            .InnerSizingV(InnerSizing.HorizontalWeight)
            .PaddingV((FooterTextPadding, 0, FooterTextPadding, 0))
            .Mutate(s.TopRule);
        {
            FooterLabel(footer, "AlvorPong 0.1 · AlvorKit sample");
            Node(footer);
            FooterLabel(footer, "Enter starts a quick match");
        }

        void FooterLabel(EntMut footer, string value) =>
            Node(footer)
                .Mutate(s.MutedLabel)
                .AlignmentV(Alignment.Vertical)
                .SizeWeightTypeV(SizeWeightType.Self)
                .TextV(value);
    }

    private void StatusBar(EntMut parent)
    {
        Node(parent, out var status)
            .Mutate(s.StatusBar)
            .PaddingV(s.Metrics.StatusBarPadding);
        {
            Node(status, out var items)
                .SizeRelativeV((1, 1))
                .InnerLayoutV(InnerLayout.HorizontalList)
                .InnerSizingV(InnerSizing.HorizontalWeight)
                .InnerSpacingV(s.Metrics.StatusSpacing);
            {
                Item(items, true, () => text.Format("{0} FPS", metrics.FrameWindow.Ticks));
                Item(items, false, () => "menu");
                Node(items);
                Item(items, false, () => "AlvorKit.UI.Blend");
            }
        }

        void Item(EntMut items, bool emphasis, Func<ReadOnlySpan<char>> value) =>
            Node(items)
                .Mutate(emphasis ? s.EmphasisLabel : s.MutedLabel)
                .AlignmentV(Alignment.Vertical)
                .SizeWeightTypeV(SizeWeightType.Self)
                .TextF(value);
    }
}
