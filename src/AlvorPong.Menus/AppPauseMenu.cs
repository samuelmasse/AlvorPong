namespace AlvorPong.Menus;

/// <summary>Builds the pause modal: score readout, resume-first actions, and the resume hints.</summary>
[App]
public class AppPauseMenu(
    RootScreen screen,
    AppMatchStart matchStart,
    AppMenuButton button,
    AppMenuReturn menuReturn,
    AppStyle s)
{
    private const float PanelWidth = 300f;
    private const float ReadoutHeight = 48f;
    private const float ReadoutLineSpacing = 3f;
    private const float HintHeight = 22f;
    private const int ScoreFontSize = 20;

    public void Create(EntMut root, AppPauseView view)
    {
        Node(root, out var layer)
            .Mutate(s.ModalLayer)
            .OnPressF(view.Resume);
        {
            Node(layer, out var panel)
                .Mutate(s.ModalPanel)
                .SizeV((PanelWidth, 0))
                .SizeInnerSumRelativeV((0, 1));
            {
                Title(panel, view.Config);
                Content(panel, view);
            }
        }
    }

    private void Title(EntMut panel, MatchConfig config)
    {
        Node(panel, out var title)
            .Mutate(s.PanelTitle)
            .InnerLayoutV(InnerLayout.HorizontalList)
            .InnerSizingV(InnerSizing.HorizontalWeight)
            .PaddingV(s.Metrics.PanelTitlePadding);
        {
            Node(title)
                .Mutate(s.EmphasisText)
                .SizeWeightTypeV(SizeWeightType.Self)
                .SizeRelativeV((0, 1))
                .SizeTextRelativeV((1, 0))
                .TextV("Paused");

            Node(title);

            Node(title)
                .Mutate(s.MutedText)
                .SizeWeightTypeV(SizeWeightType.Self)
                .SizeRelativeV((0, 1))
                .SizeTextRelativeV((1, 0))
                .TextAlignmentV(Alignment.Right | Alignment.Vertical)
                .TextPaddingV((0, 0, s.Metrics.RightGlyphPadding, 0))
                .TextV(ModeText(config));
        }
    }

    private void Content(EntMut panel, AppPauseView view)
    {
        Node(panel, out var content)
            .Mutate(s.ModalContent)
            .SizeWeightTypeV(SizeWeightType.Self)
            .SizeRelativeV((1, 0))
            .SizeInnerSumRelativeV((0, 1))
            .InnerSpacingV(s.Metrics.LooseSpacing);
        {
            ScoreReadout(content, view.Score);

            button.Create(content, "Resume", "Esc", true, view.Resume);
            button.Create(content, "Rematch", null, false, () => matchStart.Run(view.Config));
            button.Create(content, "Main menu", null, false, menuReturn.Run);
            button.Create(content, "Quit", null, false, screen.Close);

            Node(content)
                .Mutate(s.MutedText)
                .AlignmentV(Alignment.Horizontal)
                .SizeWeightTypeV(SizeWeightType.Self)
                .SizeRelativeV((0, 0))
                .SizeTextRelativeV((1, 0))
                .SizeV((0, HintHeight))
                .TextV("Esc or a click outside resumes");
        }
    }

    private void ScoreReadout(EntMut parent, MatchScore score)
    {
        Node(parent, out var readout)
            .Mutate(s.Board)
            .SizeWeightTypeV(SizeWeightType.Self)
            .SizeRelativeV((1, 0))
            .SizeV((0, ReadoutHeight))
            .ColorV(s.Palette.AppBackground)
            .Mutate(s.Border);
        {
            Node(readout, out var lines)
                .Mutate(s.VerticalList)
                .AlignmentV(Alignment.Horizontal | Alignment.Vertical)
                .InnerSpacingV(ReadoutLineSpacing);
            {
                Node(lines, out var numbers)
                    .Mutate(s.HorizontalList)
                    .AlignmentV(Alignment.Horizontal)
                    .InnerSpacingV(s.Metrics.LooseSpacing);
                {
                    Number(numbers, $"{score.Left}", s.EmphasisLabel);
                    Number(numbers, ":", s.MutedLabel);
                    Number(numbers, $"{score.Right}", s.EmphasisLabel);
                }

                Node(lines)
                    .Mutate(s.MutedLabel)
                    .AlignmentV(Alignment.Horizontal)
                    .TextV(SubText(score));
            }
        }

        static void Number(EntMut numbers, string value, Action<EntMut> style) =>
            Node(numbers)
                .Mutate(style)
                .AlignmentV(Alignment.Vertical)
                .FontSizeV(ScoreFontSize)
                .TextV(value);
    }

    private static string ModeText(MatchConfig config) =>
        config.RightIsAi
            ? $"vs AI · first to {MatchScore.WinScore}"
            : $"two players · first to {MatchScore.WinScore}";

    private static string SubText(MatchScore score)
    {
        var lead =
            score.Left > score.Right ? $"left leads by {score.Left - score.Right}"
            : score.Right > score.Left ? $"right leads by {score.Right - score.Left}"
            : $"tied at {score.Left}";
        return $"point {score.Left + score.Right + 1} · {lead}";
    }
}
