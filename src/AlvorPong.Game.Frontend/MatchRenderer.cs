namespace AlvorPong.Game.Frontend;

/// <summary>Draws the match with sprite quads and text, letterboxed into the current canvas.</summary>
[Match]
public class MatchRenderer(RootCanvas canvas, RootSprites sprites, MatchFonts fonts, MatchField field, MatchScore score)
{
    private static readonly Vec4 Foreground = (0.92f, 0.94f, 0.96f, 1f);
    private static readonly Vec4 Faint = (0.92f, 0.94f, 0.96f, 0.25f);
    private static readonly string[] DigitTexts =
    [
        "0",
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
    ];
    private static readonly string[] ScoreTexts = CreateScoreTexts();

    private Vec2 origin;
    private float scale;

    public void Draw()
    {
        scale = MathF.Min(canvas.Size.X / MatchField.Width, canvas.Size.Y / MatchField.Height);
        origin = ((Vec2)canvas.Size - new Vec2(MatchField.Width, MatchField.Height) * scale) / 2;

        const float dashLength = 26f;
        for (float y = dashLength / 2; y < MatchField.Height; y += dashLength * 2)
            Rect((MatchField.Width / 2 - 3f, y), (6f, dashLength), Faint);

        Rect((MatchField.PaddleMargin, field.LeftPaddleY - MatchField.PaddleHeight / 2),
            (MatchField.PaddleWidth, MatchField.PaddleHeight), Foreground);
        Rect((MatchField.Width - MatchField.PaddleMargin - MatchField.PaddleWidth, field.RightPaddleY - MatchField.PaddleHeight / 2),
            (MatchField.PaddleWidth, MatchField.PaddleHeight), Foreground);

        if (field.IsServing)
            DrawServeCountdown();
        else
            Rect(field.BallPosition - new Vec2(MatchField.BallSize / 2), new(MatchField.BallSize), Foreground);

        var font = fonts.Score;
        var text = ScoreText(score.Left, score.Right);
        var width = sprites.Batch.Measure(font, text);
        sprites.Batch.Write(font, text, (canvas.Size.X / 2 - width / 2, origin.Y + 24f * scale), Faint);
    }

    private void DrawServeCountdown()
    {
        var font = fonts.Countdown;
        var text = DigitTexts[Math.Clamp(field.ServeSecondsLeft, 0, DigitTexts.Length - 1)];
        var center = origin + new Vec2(MatchField.Width, MatchField.Height) * scale / 2;
        var width = sprites.Batch.Measure(font, text);

        sprites.Batch.Write(font, text, center - (width / 2, font.Metrics.Height / 2), Foreground);
    }

    private void Rect(Vec2 position, Vec2 size, Vec4 color) =>
        sprites.Batch.Draw(origin + position * scale, size * scale, color);

    private static string ScoreText(int left, int right) =>
        ScoreTexts[Math.Clamp(left, 0, 9) * 10 + Math.Clamp(right, 0, 9)];

    private static string[] CreateScoreTexts()
    {
        var texts = new string[100];
        for (var left = 0; left <= 9; left++)
        {
            for (var right = 0; right <= 9; right++)
                texts[left * 10 + right] = $"{left}   {right}";
        }

        return texts;
    }
}
