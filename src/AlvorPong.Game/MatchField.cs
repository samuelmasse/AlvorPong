namespace AlvorPong.Game;

/// <summary>Simulates the Pong field on a fixed logical field: paddles, swept ball flight, bounces, and goals.</summary>
[Match]
public class MatchField
{
    public const float Width = 1600f;
    public const float Height = 900f;
    public const float PaddleWidth = 24f;
    public const float PaddleHeight = 180f;
    public const float PaddleMargin = 48f;
    public const float BallSize = 24f;
    public const float PaddleSpeed = 1000f;

    /// <summary>The largest paddle-center-to-ball-center distance that still counts as a hit.</summary>
    public const float PaddleReach = (PaddleHeight + BallSize) / 2f;

    public const float MaxSpeed = 1700f;

    private const float ServeSpeed = 700f;
    private const float HitSpeedup = 1.05f;
    private const float SliceSpeedup = 0.06f;
    private const float MaxBounceAngle = 1.0f;
    private const float SliceAngle = 0.35f;
    private const float MaxShotAngle = 1.2f;
    private const float ServeAngle = 0.45f;
    private const double ServeDelay = 3.0;

    private double serveCountdown = ServeDelay;
    private MatchSide serveTowards = Random.Shared.Next(2) == 0 ? MatchSide.Left : MatchSide.Right;

    public float LeftPaddleY { get; private set; } = Height / 2;
    public float RightPaddleY { get; private set; } = Height / 2;
    public float LeftPaddleVelocity { get; private set; }
    public float RightPaddleVelocity { get; private set; }
    public Vec2 BallPosition { get; private set; } = (Width / 2, Height / 2);
    public Vec2 BallVelocity { get; private set; }

    /// <summary>Gets the ball-center x at which the ball contacts the left paddle face.</summary>
    public static float LeftContactX => PaddleMargin + PaddleWidth + BallSize / 2f;

    /// <summary>Gets the ball-center x at which the ball contacts the right paddle face.</summary>
    public static float RightContactX => Width - PaddleMargin - PaddleWidth - BallSize / 2f;

    /// <summary>Gets whether the ball is waiting on the serve countdown.</summary>
    public bool IsServing => serveCountdown > 0;

    /// <summary>Gets the whole seconds left before the next serve, for the countdown display.</summary>
    public int ServeSecondsLeft => (int)Math.Ceiling(serveCountdown);

    /// <summary>Computes a contact's outgoing angle: the contact offset picks the base angle, the paddle's slice bends it further.</summary>
    public static float ShotAngle(float offset, float slice) =>
        Math.Clamp(offset * MaxBounceAngle + slice * SliceAngle, -MaxShotAngle, MaxShotAngle);

    /// <summary>Computes a contact's outgoing speed: every hit speeds the ball up, slicing adds a smash bonus.</summary>
    public static float ShotSpeed(float currentSpeed, float slice) =>
        MathF.Min(currentSpeed * (HitSpeedup + MathF.Abs(slice) * SliceSpeedup), MaxSpeed);

    /// <summary>Predicts the ball-center y when it reaches <paramref name="x" />, reflecting off the field edges.</summary>
    public static float PredictBallY(Vec2 position, Vec2 velocity, float x)
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

    /// <summary>Recenters the ball and serves it toward a side after the countdown.</summary>
    public void Reset(MatchSide towards)
    {
        BallPosition = (Width / 2, Height / 2);
        BallVelocity = default;
        serveTowards = towards;
        serveCountdown = ServeDelay;
    }

    /// <summary>Advances the simulation one tick and returns the side that scored, if any.</summary>
    public MatchSide? Step(double delta, float leftAxis, float rightAxis)
    {
        float dt = (float)delta;

        LeftPaddleY = MovePaddle(LeftPaddleY, leftAxis, dt, out float leftVelocity);
        RightPaddleY = MovePaddle(RightPaddleY, rightAxis, dt, out float rightVelocity);
        LeftPaddleVelocity = leftVelocity;
        RightPaddleVelocity = rightVelocity;

        if (IsServing)
        {
            serveCountdown -= delta;
            if (serveCountdown <= 0)
                Serve();
            return null;
        }

        var previous = BallPosition;
        BallPosition += BallVelocity * dt;

        BounceOffPaddle(previous, dt);
        ReflectOffFieldEdges();

        if (BallPosition.X < -BallSize)
            return MatchSide.Right;
        if (BallPosition.X > Width + BallSize)
            return MatchSide.Left;
        return null;
    }

    private static float MovePaddle(float center, float axis, float dt, out float velocity)
    {
        float moved = Math.Clamp(center + axis * PaddleSpeed * dt, PaddleHeight / 2, Height - PaddleHeight / 2);
        velocity = dt > 0 ? (moved - center) / dt : 0f;
        return moved;
    }

    private void Serve()
    {
        float angle = (Random.Shared.NextSingle() * 2f - 1f) * ServeAngle;
        float direction = serveTowards == MatchSide.Left ? -1f : 1f;
        BallVelocity = (direction * ServeSpeed * MathF.Cos(angle), ServeSpeed * MathF.Sin(angle));
    }

    /// <summary>Sweeps the ball's motion against the facing paddle plane so fast balls cannot tunnel through.</summary>
    private void BounceOffPaddle(Vec2 previous, float dt)
    {
        if (BallVelocity.X < 0)
            TrySweptBounce(previous, dt, LeftContactX, LeftPaddleY, LeftPaddleVelocity, 1f);
        else if (BallVelocity.X > 0)
            TrySweptBounce(previous, dt, RightContactX, RightPaddleY, RightPaddleVelocity, -1f);
    }

    private void TrySweptBounce(Vec2 previous, float dt, float faceX, float paddleCenter, float paddleVelocity, float outDirection)
    {
        bool crossed = outDirection > 0
            ? previous.X >= faceX && BallPosition.X < faceX
            : previous.X <= faceX && BallPosition.X > faceX;
        if (!crossed)
            return;

        float travel = BallPosition.X - previous.X;
        float t = travel == 0 ? 0f : (faceX - previous.X) / travel;
        float crossY = previous.Y + (BallPosition.Y - previous.Y) * t;

        float offset = (crossY - paddleCenter) / PaddleReach;
        if (MathF.Abs(offset) > 1f)
            return;

        float slice = Math.Clamp(paddleVelocity / PaddleSpeed, -1f, 1f);
        float angle = ShotAngle(offset, slice);
        float speed = ShotSpeed(BallVelocity.Length, slice);
        BallVelocity = (outDirection * speed * MathF.Cos(angle), speed * MathF.Sin(angle));

        float remaining = (1f - t) * dt;
        BallPosition = new Vec2(faceX, crossY) + BallVelocity * remaining;
    }

    /// <summary>Reflects both velocity and overshoot position off the top and bottom field edges.</summary>
    private void ReflectOffFieldEdges()
    {
        float half = BallSize / 2;

        if (BallPosition.Y < half && BallVelocity.Y < 0)
        {
            BallPosition = (BallPosition.X, half + (half - BallPosition.Y));
            BallVelocity = (BallVelocity.X, -BallVelocity.Y);
        }

        float bottom = Height - half;
        if (BallPosition.Y > bottom && BallVelocity.Y > 0)
        {
            BallPosition = (BallPosition.X, bottom - (BallPosition.Y - bottom));
            BallVelocity = (BallVelocity.X, -BallVelocity.Y);
        }
    }
}
