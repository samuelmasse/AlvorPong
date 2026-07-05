namespace AlvorPong.Game;

/// <summary>Draws the match with sprite quads and text, letterboxed into the current canvas.</summary>
[Match]
public class MatchRenderer(RootCanvas canvas, RootSprites sprites, RootRoboto roboto, MatchField field, MatchScore score)
{
    private static readonly Vec4 Foreground = (0.92f, 0.94f, 0.96f, 1f);
    private static readonly Vec4 Faint = (0.92f, 0.94f, 0.96f, 0.25f);

    public void Draw()
    {
        float scale = MathF.Min(canvas.Size.X / MatchField.Width, canvas.Size.Y / MatchField.Height);
        var origin = ((Vec2)canvas.Size - new Vec2(MatchField.Width, MatchField.Height) * scale) / 2;

        void Rect(Vec2 position, Vec2 size, Vec4 color) =>
            sprites.Batch.Draw(origin + position * scale, size * scale, color);

        const float dashLength = 26f;
        for (float y = dashLength / 2; y < MatchField.Height; y += dashLength * 2)
            Rect((MatchField.Width / 2 - 3f, y), (6f, dashLength), Faint);

        Rect((MatchField.PaddleMargin, field.LeftPaddleY - MatchField.PaddleHeight / 2),
            (MatchField.PaddleWidth, MatchField.PaddleHeight), Foreground);
        Rect((MatchField.Width - MatchField.PaddleMargin - MatchField.PaddleWidth, field.RightPaddleY - MatchField.PaddleHeight / 2),
            (MatchField.PaddleWidth, MatchField.PaddleHeight), Foreground);

        if (field.IsServing)
            DrawServeCountdown(origin, scale);
        else
            Rect(field.BallPosition - new Vec2(MatchField.BallSize / 2), new(MatchField.BallSize), Foreground);

        var font = roboto[54];
        var scoreText = $"{score.Left}   {score.Right}";
        float width = sprites.Batch.Measure(font, scoreText);
        sprites.Batch.Write(font, scoreText, (canvas.Size.X / 2 - width / 2, origin.Y + 24f * scale), Faint);
    }

    private void DrawServeCountdown(Vec2 origin, float scale)
    {
        var font = roboto[140];
        var text = $"{field.ServeSecondsLeft}";
        var center = origin + new Vec2(MatchField.Width, MatchField.Height) * scale / 2;

        float width = sprites.Batch.Measure(font, text);
        sprites.Batch.Write(font, text, center - (width / 2, font.Metrics.Height / 2), Foreground);
    }
}
