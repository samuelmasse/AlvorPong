namespace AlvorPong.Game;

/// <summary>Advances ready paddles from their component movement intent.</summary>
[Match]
public class MatchPaddleMovementSystem(MatchPaddleBag paddles)
{
    /// <summary>Moves every ready paddle and records its realized velocity.</summary>
    public void Update(double delta)
    {
        float dt = (float)delta;
        var ents = paddles.Ents;
        for (var i = 0; i < ents.Length; i++)
        {
            var paddle = ents[i];
            var position = paddle.Position;
            float y = Math.Clamp(
                position.Y + paddle.MovementAxis * MatchField.PaddleSpeed * dt,
                MatchField.PaddleHeight / 2f,
                MatchField.Height - MatchField.PaddleHeight / 2f);

            paddle.Position = (position.X, y);
            paddle.Velocity = (0f, dt > 0 ? (y - position.Y) / dt : 0f);
        }
    }
}
