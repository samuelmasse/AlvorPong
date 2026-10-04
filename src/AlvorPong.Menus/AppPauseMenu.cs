namespace AlvorPong;

/// <summary>Builds the pause modal: score readout, resume-first actions, and the resume hints.</summary>
[App]
public class AppPauseMenu(
    RootScreen screen,
    AppMatchStart matchStart,
    AppMenuButton button,
    AppMenuReturn menuReturn,
    BlendUi bl)
{
    public void Create(EntMut root, AppPauseView view)
    {
        const float panelWidth = 300f;
        const float readoutHeight = 48f;
        const float readoutLineSpacing = 3f;
        const float hintHeight = 22f;
        const int scoreFontSize = 20;

        Node(root, out var layer)
            .Mutate(bl.S.ModalLayer)
            .OnPressF(view.Resume);
        {
            Node(layer, out var panel)
                .Mutate(bl.S.ModalPanel)
                .SizeV((panelWidth, 0))
                .SizeInnerSumRelativeV((0, 1));
            {
                Title(panel);
                Content(panel);
            }
        }

        void Title(EntMut panel)
        {
            Node(panel, out var title)
                .Mutate(bl.S.PanelTitle)
                .InnerLayoutV(InnerLayout.HorizontalList)
                .InnerSizingV(InnerSizing.HorizontalWeight)
                .PaddingV(bl.S.Metrics.PanelTitlePadding);
            {
                Node(title)
                    .Mutate(bl.S.EmphasisText)
                    .SizeWeightTypeV(SizeWeightType.Self)
                    .SizeRelativeV((0, 1))
                    .SizeTextRelativeV((1, 0))
                    .TextV("Paused");

                Node(title);

                Node(title)
                    .Mutate(bl.S.MutedText)
                    .SizeWeightTypeV(SizeWeightType.Self)
                    .SizeRelativeV((0, 1))
                    .SizeTextRelativeV((1, 0))
                    .TextAlignmentV(Alignment.Right | Alignment.Vertical)
                    .TextPaddingV((0, 0, bl.S.Metrics.RightGlyphPadding, 0))
                    .TextV(ModeText());
            }
        }

        string ModeText() =>
            view.Config.RightIsAi
                ? $"vs AI · first to {MatchScore.WinScore}"
                : $"two players · first to {MatchScore.WinScore}";

        void Content(EntMut panel)
        {
            Node(panel, out var content)
                .Mutate(bl.S.ModalContent)
                .SizeWeightTypeV(SizeWeightType.Self)
                .SizeRelativeV((1, 0))
                .SizeInnerSumRelativeV((0, 1))
                .InnerSpacingV(bl.S.Metrics.LooseSpacing);
            {
                ScoreReadout(content);

                button.Create(content, "Resume", "Esc", true, view.Resume);
                button.Create(content, "Rematch", null, false, () => matchStart.Run(view.Config));
                button.Create(content, "Main menu", null, false, menuReturn.Run);
                button.Create(content, "Quit", null, false, screen.Close);

                Node(content)
                    .Mutate(bl.S.MutedText)
                    .AlignmentV(Alignment.Horizontal)
                    .SizeWeightTypeV(SizeWeightType.Self)
                    .SizeRelativeV((0, 0))
                    .SizeTextRelativeV((1, 0))
                    .SizeV((0, hintHeight))
                    .TextV("Esc or a click outside resumes");
            }
        }

        void ScoreReadout(EntMut parent)
        {
            Node(parent, out var readout)
                .Mutate(bl.S.Board)
                .SizeWeightTypeV(SizeWeightType.Self)
                .SizeRelativeV((1, 0))
                .SizeV((0, readoutHeight))
                .ColorV(bl.S.Palette.AppBackground)
                .Mutate(bl.S.Border);
            {
                Node(readout, out var lines)
                    .Mutate(bl.S.VerticalList)
                    .AlignmentV(Alignment.Horizontal | Alignment.Vertical)
                    .InnerSpacingV(readoutLineSpacing);
                {
                    Node(lines, out var numbers)
                        .Mutate(bl.S.HorizontalList)
                        .AlignmentV(Alignment.Horizontal)
                        .InnerSpacingV(bl.S.Metrics.LooseSpacing);
                    {
                        Number(numbers, $"{view.Score.Left}", bl.S.EmphasisLabel);
                        Number(numbers, ":", bl.S.MutedLabel);
                        Number(numbers, $"{view.Score.Right}", bl.S.EmphasisLabel);
                    }

                    Node(lines)
                        .Mutate(bl.S.MutedLabel)
                        .AlignmentV(Alignment.Horizontal)
                        .TextV(SubText());
                }
            }
        }

        void Number(EntMut parent, string value, Action<EntMut> style) =>
            Node(parent)
                .Mutate(style)
                .AlignmentV(Alignment.Vertical)
                .FontSizeV(scoreFontSize)
                .TextV(value);

        string SubText()
        {
            var score = view.Score;
            var lead =
                score.Left > score.Right ? $"left leads by {score.Left - score.Right}"
                : score.Right > score.Left ? $"right leads by {score.Right - score.Left}"
                : $"tied at {score.Left}";
            return $"point {score.Left + score.Right + 1} · {lead}";
        }
    }
}
