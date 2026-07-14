namespace AlvorPong.Menus;

/// <summary>Builds the game-over card: winner, scoreboard, point tape, and rematch-first actions.</summary>
[App]
public class AppGameOverMenu(
    AppMatchStart matchStart,
    AppMenuButton button,
    AppMenuReturn menuReturn,
    AppStyle s)
{
    public void Create(EntMut root, AppGameOverView view)
    {
        const float panelWidth = 400f;
        const float winnerCapHeight = 14f;
        const float winnerNameHeight = 30f;
        const float numberRowHeight = 46f;
        const float numberRuleWidth = 34f;
        const float numberSpacing = 18f;
        const float numberRuleSpacing = 6f;
        const float tapePointSize = 10f;
        const float tapeSpacing = 3f;
        const float captionHeight = 14f;
        const float boardMarginTop = 4f;
        const float tapeMarginTop = 8f;
        const float rematchMarginTop = 10f;
        const int winnerNameFontSize = 22;
        const int numberFontSize = 44;
        const int colonFontSize = 30;

        Node(root, out var layer)
            .Mutate(s.ModalLayer);
        {
            Node(layer, out var panel)
                .Mutate(s.ModalPanel)
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
                    .TextV(ModeText());
            }
        }

        string ModeText() =>
            view.Config.RightIsAi
                ? $"vs AI · {view.Score.History.Length} points"
                : $"two players · {view.Score.History.Length} points";

        void Content(EntMut panel)
        {
            Node(panel, out var content)
                .Mutate(s.ModalContent)
                .SizeWeightTypeV(SizeWeightType.Self)
                .SizeRelativeV((1, 0))
                .SizeInnerSumRelativeV((0, 1))
                .InnerSpacingV(s.Metrics.LooseSpacing);
            {
                Line(content, "WINNER", s.MutedText, s.Metrics.MutedFontSize, winnerCapHeight);
                Line(content, WinnerName(), s.EmphasisText, winnerNameFontSize, winnerNameHeight);
                Scoreboard(content);
                Tape(content);
                Line(content, "point history — left · right", s.MutedText, s.Metrics.MutedFontSize, captionHeight);

                button.Create(content, "Rematch", "Enter", true, () => matchStart.Run(view.Config))
                    .Mutate()
                    .MarginV((0, rematchMarginTop, 0, 0));
                button.Create(content, "Main menu", "Esc", false, menuReturn.Run);
            }
        }

        string WinnerName() =>
            view.Score.Winner == MatchSide.Left ? "Left player"
            : view.Config.RightIsAi ? "AI"
            : "Right player";

        void Line(EntMut parent, string value, Action<EntMut> style, int fontSize, float height) =>
            Node(parent)
                .Mutate(style)
                .AlignmentV(Alignment.Horizontal)
                .SizeRelativeV((0, 0))
                .SizeTextRelativeV((1, 0))
                .SizeV((0, height))
                .FontSizeV(fontSize)
                .TextV(value);

        void Scoreboard(EntMut parent)
        {
            var score = view.Score;
            Node(parent, out var board)
                .Mutate(s.HorizontalList)
                .AlignmentV(Alignment.Horizontal)
                .MarginV((0, boardMarginTop, 0, 0))
                .InnerSpacingV(numberSpacing);
            {
                NumberColumn(board, score.Left, score.Winner == MatchSide.Left);

                Node(board)
                    .Mutate(s.MutedText)
                    .AlignmentV(Alignment.Top)
                    .SizeRelativeV((0, 0))
                    .SizeTextRelativeV((1, 0))
                    .SizeV((0, numberRowHeight))
                    .FontSizeV(colonFontSize)
                    .TextV(":");

                NumberColumn(board, score.Right, score.Winner == MatchSide.Right);
            }
        }

        void NumberColumn(EntMut board, int value, bool winner)
        {
            Node(board, out var column)
                .Mutate(s.VerticalList)
                .AlignmentV(Alignment.Top)
                .InnerSpacingV(numberRuleSpacing);
            {
                Node(column, out var number)
                    .Mutate(s.EmphasisText)
                    .AlignmentV(Alignment.Horizontal)
                    .SizeRelativeV((0, 0))
                    .SizeTextRelativeV((1, 0))
                    .SizeV((0, numberRowHeight))
                    .FontSizeV(numberFontSize)
                    .TextV($"{value}");
                if (!winner)
                {
                    number.Mutate()
                        .TextColorV(s.Palette.MutedText);
                }

                Node(column)
                    .AlignmentV(Alignment.Horizontal)
                    .SizeRelativeV((0, 0))
                    .SizeV((numberRuleWidth, s.Metrics.ActiveTabAccentHeight))
                    .ColorV(winner ? s.Palette.Accent : default);
            }
        }

        void Tape(EntMut parent)
        {
            Node(parent, out var tape)
                .Mutate(s.HorizontalList)
                .AlignmentV(Alignment.Horizontal)
                .MarginV((0, tapeMarginTop, 0, 0))
                .InnerSpacingV(tapeSpacing)
                .SizeV((0, tapePointSize));
            {
                foreach (var side in view.Score.History)
                {
                    Node(tape)
                        .AlignmentV(Alignment.Vertical)
                        .SizeRelativeV((0, 0))
                        .SizeV((tapePointSize, tapePointSize))
                        .ColorV(side == MatchSide.Left ? s.TapeLeftPoint : s.TapeRightPoint);
                }
            }
        }
    }
}
