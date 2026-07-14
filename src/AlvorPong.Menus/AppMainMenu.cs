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
    public void Create(EntMut root)
    {
        const float splashWidth = 560f;
        const float bannerHeight = 120f;
        const float footerHeight = 30f;
        const float footerTextPadding = 14f;
        const float bodyPadding = 16f;
        const float actionColumnWidth = 250f;
        const float controlsPaddingX = 12f;
        const float controlsPaddingY = 10f;
        const float controlsCaptionHeight = 21f;
        const float controlRowHeight = 24f;
        const float controlValueColumn = 104f;
        const float brandmarkSize = 16f;
        const float wordSpacing = 12f;
        const float wordLineSpacing = 7f;
        const int wordmarkFontSize = 34;
        const int captionFontSize = 10;

        Node(root, out var panel)
            .Mutate(s.Root);
        {
            Node(panel)
                .ColorV(default);

            Splash(panel);

            Node(panel)
                .ColorV(default);

            StatusBar(panel);
        }

        void Splash(EntMut parent)
        {
            Node(parent, out var splash)
                .AlignmentV(Alignment.Horizontal)
                .SizeWeightTypeV(SizeWeightType.Self)
                .SizeRelativeV((0, 0))
                .SizeInnerSumRelativeV((0, 1))
                .SizeV((splashWidth, 0))
                .InnerLayoutV(InnerLayout.VerticalList)
                .ColorV(s.Palette.Panel)
                .Mutate(s.StrongBorder);
            {
                Banner(splash);
                Body(splash);
                Footer(splash);
            }
        }

        void Banner(EntMut parent)
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
                .SizeV((0, bannerHeight))
                .ColorV(s.Palette.AppBackground)
                .Mutate(s.BottomRule);
            {
                for (var i = 0; i < dashCount; i++)
                {
                    Node(banner)
                        .OffsetV(((splashWidth - dashWidth) / 2, dashStartY + i * dashStep))
                        .SizeRelativeV((0, 0))
                        .SizeV((dashWidth, dashHeight))
                        .ColorV(s.FieldFaint);
                }

                FieldRect(banner, (paddleInset, 30), (paddleWidth, paddleHeight));
                FieldRect(banner, (splashWidth - paddleInset - paddleWidth, 44), (paddleWidth, paddleHeight));
                FieldRect(banner, (462, 60), (ballSize, ballSize));

                Node(banner, out var word)
                    .Mutate(s.VerticalList)
                    .AlignmentV(Alignment.Horizontal | Alignment.Vertical)
                    .InnerSpacingV(wordLineSpacing);
                {
                    Node(word, out var wordRow)
                        .Mutate(s.HorizontalList)
                        .AlignmentV(Alignment.Horizontal)
                        .InnerSpacingV(wordSpacing);
                    {
                        Node(wordRow)
                            .AlignmentV(Alignment.Vertical)
                            .SizeRelativeV((0, 0))
                            .SizeV((brandmarkSize, brandmarkSize))
                            .ColorV(s.Palette.ActiveSurface)
                            .Mutate(s.AccentBorder);

                        Node(wordRow)
                            .Mutate(s.EmphasisLabel)
                            .AlignmentV(Alignment.Vertical)
                            .FontSizeV(wordmarkFontSize)
                            .TextV("ALVORPONG");
                    }

                    Node(word)
                        .Mutate(s.MutedLabel)
                        .AlignmentV(Alignment.Horizontal)
                        .TextV("first to five — Pong on the AlvorKit engine");
                }
            }
        }

        void FieldRect(EntMut parent, Vec2 offset, Vec2 size) =>
            Node(parent)
                .OffsetV(offset)
                .SizeRelativeV((0, 0))
                .SizeV(size)
                .ColorV(s.FieldFaint);

        float ActionColumnHeight() =>
            4 * s.Metrics.ButtonHeight + 4 * s.Metrics.LooseSpacing + s.Metrics.Hairline;

        void Body(EntMut parent)
        {
            Node(parent, out var body)
                .SizeRelativeV((1, 0))
                .SizeV((0, ActionColumnHeight() + bodyPadding * 2))
                .InnerLayoutV(InnerLayout.HorizontalList)
                .InnerSpacingV(bodyPadding)
                .PaddingV((bodyPadding, bodyPadding, bodyPadding, bodyPadding));
            {
                Actions(body);
                Controls(body);
            }
        }

        void Actions(EntMut parent)
        {
            Node(parent, out var actions)
                .Mutate(s.VerticalList)
                .SizeInnerMaxRelativeV((0, 0))
                .SizeV((actionColumnWidth, 0))
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

        void Controls(EntMut parent)
        {
            var width = splashWidth - bodyPadding * 3 - actionColumnWidth;
            Node(parent, out var card)
                .SizeRelativeV((0, 0))
                .SizeV((width, ActionColumnHeight()))
                .InnerLayoutV(InnerLayout.VerticalList)
                .PaddingV((controlsPaddingX, controlsPaddingY, controlsPaddingX, controlsPaddingY))
                .ColorV(s.Palette.AppBackground)
                .Mutate(s.Border);
            {
                Node(card, out var caption)
                    .SizeRelativeV((1, 0))
                    .SizeV((0, controlsCaptionHeight))
                    .InnerLayoutV(InnerLayout.HorizontalList)
                    .InnerSizingV(InnerSizing.HorizontalWeight);
                {
                    CaptionLabel(caption, "CONTROLS");
                    Node(caption);
                    CaptionLabel(caption, "right is AI in solo");
                }

                KeyRow(card, "left paddle", "W", "S");
                KeyRow(card, "right paddle", "I", "K");
                KeyRow(card, "pause", "Esc", null);
                TextRow(card, "win", "first to 5 points");
            }
        }

        void CaptionLabel(EntMut parent, string value) =>
            Node(parent)
                .Mutate(s.MutedLabel)
                .AlignmentV(Alignment.Vertical)
                .SizeWeightTypeV(SizeWeightType.Self)
                .FontSizeV(captionFontSize)
                .TextV(value);

        EntMut ControlRow(EntMut parent, string label)
        {
            Node(parent, out var row)
                .Mutate(s.MetricRow)
                .SizeV((0, controlRowHeight));
            {
                Node(row)
                    .Mutate(s.MutedLabel)
                    .AlignmentV(Alignment.Left | Alignment.Vertical)
                    .TextV(label);
            }

            return row;
        }

        void KeyRow(EntMut parent, string label, string firstKey, string? secondKey)
        {
            var row = ControlRow(parent, label);
            Node(row, out var value)
                .Mutate(s.HorizontalList)
                .AlignmentV(Alignment.Left | Alignment.Vertical)
                .OffsetV((controlValueColumn, 0))
                .InnerSpacingV(s.Metrics.CompactSpacing);
            {
                Node(value)
                    .Mutate(s.KeyChip)
                    .AlignmentV(Alignment.Vertical)
                    .TextV(firstKey);

                if (secondKey != null)
                {
                    Node(value)
                        .Mutate(s.MutedLabel)
                        .AlignmentV(Alignment.Vertical)
                        .TextV("/");

                    Node(value)
                        .Mutate(s.KeyChip)
                        .AlignmentV(Alignment.Vertical)
                        .TextV(secondKey);
                }
            }
        }

        void TextRow(EntMut parent, string label, string value)
        {
            var row = ControlRow(parent, label);
            Node(row)
                .Mutate(s.MutedLabel)
                .AlignmentV(Alignment.Left | Alignment.Vertical)
                .OffsetV((controlValueColumn, 0))
                .TextV(value);
        }

        void Footer(EntMut parent)
        {
            Node(parent, out var footer)
                .SizeRelativeV((1, 0))
                .SizeV((0, footerHeight))
                .InnerLayoutV(InnerLayout.HorizontalList)
                .InnerSizingV(InnerSizing.HorizontalWeight)
                .PaddingV((footerTextPadding, 0, footerTextPadding, 0))
                .Mutate(s.TopRule);
            {
                FooterLabel(footer, "AlvorPong 0.1 · AlvorKit sample");
                Node(footer);
                FooterLabel(footer, "Enter starts a quick match");
            }
        }

        void FooterLabel(EntMut parent, string value) =>
            Node(parent)
                .Mutate(s.MutedLabel)
                .AlignmentV(Alignment.Vertical)
                .SizeWeightTypeV(SizeWeightType.Self)
                .TextV(value);

        void StatusBar(EntMut parent)
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
        }

        void Item(EntMut parent, bool emphasis, Func<ReadOnlySpan<char>> value) =>
            Node(parent)
                .Mutate(emphasis ? s.EmphasisLabel : s.MutedLabel)
                .AlignmentV(Alignment.Vertical)
                .SizeWeightTypeV(SizeWeightType.Self)
                .TextF(value);
    }
}
