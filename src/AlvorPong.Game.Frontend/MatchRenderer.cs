namespace AlvorPong.Game.Frontend;

/// <summary>Draws ECS match Ents with sprite quads and text, letterboxed into the current canvas.</summary>
[Match]
public class MatchRenderer(
    RootCanvas canvas,
    RootSprites sprites,
    MatchFonts fonts,
    MatchScore score,
    MatchScoreText scoreText,
    MatchServe serve,
    MatchPaddleBag paddles,
    MatchBallBag balls)
{
    private readonly Vec4 foreground = (0.92f, 0.94f, 0.96f, 1f);
    private readonly Vec4 faint = (0.92f, 0.94f, 0.96f, 0.25f);

    /// <summary>Draws the current match frame from ready ECS bags.</summary>
    public void Draw()
    {
        var scale = MathF.Min(canvas.Size.X / MatchField.Width, canvas.Size.Y / MatchField.Height);
        Vec2 fieldSize = (MatchField.Width, MatchField.Height);
        var origin = ((Vec2)canvas.Size - fieldSize * scale) / 2;

        const float dashLength = 26f;
        for (float y = dashLength / 2; y < MatchField.Height; y += dashLength * 2)
            Rect(origin, scale, (MatchField.Width / 2 - 3f, y), (6f, dashLength), faint);

        foreach (var paddle in paddles.Ents)
        {
            Rect(
                origin,
                scale,
                paddle.Position - new Vec2(MatchField.PaddleWidth / 2f, MatchField.PaddleHeight / 2f),
                (MatchField.PaddleWidth, MatchField.PaddleHeight),
                foreground);
        }

        if (serve.IsServing)
        {
            DrawServeCountdown(origin, scale);
        }
        else
        {
            foreach (var ball in balls.Ents)
            {
                Rect(
                    origin,
                    scale,
                    ball.Position - new Vec2(MatchField.BallSize / 2f),
                    new(MatchField.BallSize),
                    foreground);
            }
        }

        var font = fonts.Score;
        var text = scoreText.Get(score.Left, score.Right);
        var width = sprites.Batch.Measure(font, text);
        sprites.Batch.Write(font, text, (canvas.Size.X / 2 - width / 2, origin.Y + 24f * scale), faint);
    }

    private void DrawServeCountdown(Vec2 origin, float scale)
    {
        var font = fonts.Countdown;
        var text = serve.SecondsLeft switch
        {
            1 => "1",
            2 => "2",
            _ => "3",
        };
        Vec2 fieldSize = (MatchField.Width, MatchField.Height);
        var center = origin + fieldSize * scale / 2;
        var width = sprites.Batch.Measure(font, text);

        sprites.Batch.Write(font, text, center - (width / 2, font.Metrics.Height / 2), foreground);
    }

    private void Rect(Vec2 origin, float scale, Vec2 position, Vec2 size, Vec4 color) =>
        sprites.Batch.Draw(origin + position * scale, size * scale, color);
}
