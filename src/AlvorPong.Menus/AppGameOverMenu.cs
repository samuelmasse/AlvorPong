namespace AlvorPong.Menus;

/// <summary>Builds the game-over card: winner, scoreboard, point tape, and rematch-first actions.</summary>
[App]
public class AppGameOverMenu(
    AppMatchStart matchStart,
    AppMenuButton button,
    AppMenuReturn menuReturn,
    AppStyle s)
{
    private const float PanelWidth = 400f;
    private const float WinnerCapHeight = 14f;
    private const float WinnerNameHeight = 30f;
    private const float NumberRowHeight = 46f;
    private const float NumberRuleWidth = 34f;
    private const float NumberSpacing = 18f;
    private const float NumberRuleSpacing = 6f;
    private const float TapePointSize = 10f;
    private const float TapeSpacing = 3f;
    private const float CaptionHeight = 14f;
    private const float BoardMarginTop = 4f;
    private const float TapeMarginTop = 8f;
    private const float RematchMarginTop = 10f;
    private const int WinnerNameFontSize = 22;
    private const int NumberFontSize = 44;
    private const int ColonFontSize = 30;

    public void Create(EntMut root, AppGameOverView view)
    {
        Node(root, out var layer)
            .Mutate(s.ModalLayer);
        {
            Node(layer, out var panel)
                .Mutate(s.ModalPanel)
                .SizeV((PanelWidth, 0))
                .SizeInnerSumRelativeV((0, 1));
            {
                Title(panel, view);
                Content(panel, view);
            }
        }
    }

    private void Title(EntMut panel, AppGameOverView view)
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
                .TextV("Match over");

            Node(title);

            Node(title)
                .Mutate(s.MutedText)
                .SizeWeightTypeV(SizeWeightType.Self)
                .SizeRelativeV((0, 1))
                .SizeTextRelativeV((1, 0))
                .TextAlignmentV(Alignment.Right | Alignment.Vertical)
                .TextPaddingV((0, 0, s.Metrics.RightGlyphPadding, 0))
                .TextV(ModeText(view));
        }
    }

    private void Content(EntMut panel, AppGameOverView view)
    {
        Node(panel, out var content)
            .Mutate(s.ModalContent)
            .SizeWeightTypeV(SizeWeightType.Self)
            .SizeRelativeV((1, 0))
            .SizeInnerSumRelativeV((0, 1))
            .InnerSpacingV(s.Metrics.LooseSpacing);
        {
            Line(content, "WINNER", s.MutedText, s.Metrics.MutedFontSize, WinnerCapHeight);
            Line(content, WinnerName(view), s.EmphasisText, WinnerNameFontSize, WinnerNameHeight);
            Scoreboard(content, view.Score);
            Tape(content, view.Score);
            Line(content, "point history — left · right", s.MutedText, s.Metrics.MutedFontSize, CaptionHeight);

            button.Create(content, "Rematch", "Enter", true, () => matchStart.Run(view.Config))
                .Mutate()
                .MarginV((0, RematchMarginTop, 0, 0));
            button.Create(content, "Main menu", "Esc", false, menuReturn.Run);
        }

        static void Line(EntMut parent, string value, Action<EntMut> style, int fontSize, float height) =>
            Node(parent)
                .Mutate(style)
                .AlignmentV(Alignment.Horizontal)
                .SizeRelativeV((0, 0))
                .SizeTextRelativeV((1, 0))
                .SizeV((0, height))
                .FontSizeV(fontSize)
                .TextV(value);
    }

    private void Scoreboard(EntMut parent, MatchScore score)
    {
        Node(parent, out var board)
            .Mutate(s.HorizontalList)
            .AlignmentV(Alignment.Horizontal)
            .MarginV((0, BoardMarginTop, 0, 0))
            .InnerSpacingV(NumberSpacing);
        {
            NumberColumn(board, score.Left, score.Winner == MatchSide.Left);

            Node(board)
                .Mutate(s.MutedText)
                .AlignmentV(Alignment.Top)
                .SizeRelativeV((0, 0))
                .SizeTextRelativeV((1, 0))
                .SizeV((0, NumberRowHeight))
                .FontSizeV(ColonFontSize)
                .TextV(":");

            NumberColumn(board, score.Right, score.Winner == MatchSide.Right);
        }
    }

    private void NumberColumn(EntMut board, int value, bool winner)
    {
        Node(board, out var column)
            .Mutate(s.VerticalList)
            .AlignmentV(Alignment.Top)
            .InnerSpacingV(NumberRuleSpacing);
        {
            Node(column, out var number)
                .Mutate(s.EmphasisText)
                .AlignmentV(Alignment.Horizontal)
                .SizeRelativeV((0, 0))
                .SizeTextRelativeV((1, 0))
                .SizeV((0, NumberRowHeight))
                .FontSizeV(NumberFontSize)
                .TextV($"{value}");
            if (!winner)
                number.Mutate()
                    .TextColorV(s.Palette.MutedText);

            Node(column)
                .AlignmentV(Alignment.Horizontal)
                .SizeRelativeV((0, 0))
                .SizeV((NumberRuleWidth, s.Metrics.ActiveTabAccentHeight))
                .ColorV(winner ? s.Palette.Accent : default);
        }
    }

    private void Tape(EntMut parent, MatchScore score)
    {
        Node(parent, out var tape)
            .Mutate(s.HorizontalList)
            .AlignmentV(Alignment.Horizontal)
            .MarginV((0, TapeMarginTop, 0, 0))
            .InnerSpacingV(TapeSpacing)
            .SizeV((0, TapePointSize));
        {
            foreach (var side in score.History)
                Node(tape)
                    .AlignmentV(Alignment.Vertical)
                    .SizeRelativeV((0, 0))
                    .SizeV((TapePointSize, TapePointSize))
                    .ColorV(side == MatchSide.Left ? s.TapeLeftPoint : s.TapeRightPoint);
        }
    }

    private static string ModeText(AppGameOverView view) =>
        view.Config.RightIsAi
            ? $"vs AI · {view.Score.History.Count} points"
            : $"two players · {view.Score.History.Count} points";

    private static string WinnerName(AppGameOverView view) =>
        view.Score.Winner == MatchSide.Left ? "Left player"
        : view.Config.RightIsAi ? "AI"
        : "Right player";
}
