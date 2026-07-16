namespace AlvorPong.Game;

/// <summary>Defines the logical Pong field and its pure shot geometry.</summary>
[Match]
public class MatchField
{
    /// <summary>Logical field width.</summary>
    public const float Width = 1600f;

    /// <summary>Logical field height.</summary>
    public const float Height = 900f;

    /// <summary>Paddle width in logical field units.</summary>
    public const float PaddleWidth = 24f;

    /// <summary>Paddle height in logical field units.</summary>
    public const float PaddleHeight = 180f;

    /// <summary>Horizontal distance from each paddle's outer edge to the field boundary.</summary>
    public const float PaddleMargin = 48f;

    /// <summary>Ball width and height in logical field units.</summary>
    public const float BallSize = 24f;

    /// <summary>Maximum paddle movement speed in logical field units per second.</summary>
    public const float PaddleSpeed = 1000f;

    /// <summary>The largest paddle-center-to-ball-center distance that still counts as a hit.</summary>
    public const float PaddleReach = (PaddleHeight + BallSize) / 2f;

    /// <summary>Maximum ball speed in logical field units per second.</summary>
    public const float MaxSpeed = 1700f;

    private const float HitSpeedup = 1.05f;
    private const float SliceSpeedup = 0.06f;
    private const float MaxBounceAngle = 1.0f;
    private const float SliceAngle = 0.35f;
    private const float MaxShotAngle = 1.2f;

    /// <summary>Gets the ball-center x at which the ball contacts the left paddle face.</summary>
    public float LeftContactX => PaddleMargin + PaddleWidth + BallSize / 2f;

    /// <summary>Gets the ball-center x at which the ball contacts the right paddle face.</summary>
    public float RightContactX => Width - PaddleMargin - PaddleWidth - BallSize / 2f;

    /// <summary>Computes a contact's outgoing angle from its offset and the paddle's slice.</summary>
    public float ShotAngle(float offset, float slice) =>
        Math.Clamp(offset * MaxBounceAngle + slice * SliceAngle, -MaxShotAngle, MaxShotAngle);

    /// <summary>Computes the accelerated outgoing ball speed for a paddle contact.</summary>
    public float ShotSpeed(float currentSpeed, float slice) =>
        MathF.Min(currentSpeed * (HitSpeedup + MathF.Abs(slice) * SliceSpeedup), MaxSpeed);

    /// <summary>Predicts the ball-center y at an x coordinate, reflecting off the field edges.</summary>
    public float PredictBallY(Vec2 position, Vec2 velocity, float x)
    {
        if (velocity.X == 0)
            return position.Y;

        float min = BallSize / 2f;
        float span = Height - BallSize;
        float period = span * 2f;

        float y = (position.Y + velocity.Y * ((x - position.X) / velocity.X) - min) % period;
        if (y < 0)
            y += period;
        if (y > span)
            y = period - y;
        return y + min;
    }
}
