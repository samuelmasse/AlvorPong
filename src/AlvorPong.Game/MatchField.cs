namespace AlvorPong.Game;

/// <summary>Simulates the Pong field on a fixed logical field: paddles, swept ball flight, bounces, and goals.</summary>
[Match]
public class MatchField(MatchRandom random)
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
    private const float SliceSoundThreshold = 0.45f;
    private const float MaxShotAngle = 1.2f;
    private const float ServeAngle = 0.45f;
    private const double ServeDelay = 3.0;

    private float leftPaddleY = Height / 2;
    private float rightPaddleY = Height / 2;
    private float leftPaddleVelocity;
    private float rightPaddleVelocity;
    private Vec2 ballPosition = (Width / 2, Height / 2);
    private Vec2 ballVelocity;
    private double serveCountdown = ServeDelay;
    private MatchSide serveTowards = random.NextSide();

    /// <summary>Gets the left paddle center along the field's y-axis.</summary>
    public float LeftPaddleY => leftPaddleY;

    /// <summary>Gets the right paddle center along the field's y-axis.</summary>
    public float RightPaddleY => rightPaddleY;

    /// <summary>Gets the left paddle's velocity along the field's y-axis.</summary>
    public float LeftPaddleVelocity => leftPaddleVelocity;

    /// <summary>Gets the right paddle's velocity along the field's y-axis.</summary>
    public float RightPaddleVelocity => rightPaddleVelocity;

    /// <summary>Gets the ball center in field coordinates.</summary>
    public Vec2 BallPosition => ballPosition;

    /// <summary>Gets the ball velocity in field units per second.</summary>
    public Vec2 BallVelocity => ballVelocity;

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
        ballPosition = (Width / 2, Height / 2);
        ballVelocity = default;
        serveTowards = towards;
        serveCountdown = ServeDelay;
    }

    /// <summary>Advances the simulation one tick and returns the side that scored, if any.</summary>
    public MatchSide? Step(double delta, float leftAxis, float rightAxis, out MatchFieldEvents events)
    {
        events = MatchFieldEvents.None;
        float dt = (float)delta;

        leftPaddleY = MovePaddle(leftPaddleY, leftAxis, dt, out leftPaddleVelocity);
        rightPaddleY = MovePaddle(rightPaddleY, rightAxis, dt, out rightPaddleVelocity);

        if (IsServing)
        {
            serveCountdown -= delta;
            if (serveCountdown <= 0)
            {
                Serve();
                events |= MatchFieldEvents.ServeLaunch;
            }
            return null;
        }

        var previous = ballPosition;
        ballPosition += ballVelocity * dt;

        events |= BounceOffPaddle(previous, dt);
        events |= ReflectOffFieldEdges();

        if (ballPosition.X < -BallSize)
            return MatchSide.Right;
        if (ballPosition.X > Width + BallSize)
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
        float angle = random.NextSignedSingle() * ServeAngle;
        float direction = serveTowards == MatchSide.Left ? -1f : 1f;
        ballVelocity = (direction * ServeSpeed * MathF.Cos(angle), ServeSpeed * MathF.Sin(angle));
    }

    /// <summary>Sweeps the ball's motion against the facing paddle plane so fast balls cannot tunnel through.</summary>
    private MatchFieldEvents BounceOffPaddle(Vec2 previous, float dt)
    {
        if (ballVelocity.X < 0)
            return TrySweptBounce(previous, dt, LeftContactX, leftPaddleY, leftPaddleVelocity, 1f);
        if (ballVelocity.X > 0)
            return TrySweptBounce(previous, dt, RightContactX, rightPaddleY, rightPaddleVelocity, -1f);
        return MatchFieldEvents.None;
    }

    private MatchFieldEvents TrySweptBounce(Vec2 previous, float dt, float faceX, float paddleCenter, float paddleVelocity, float outDirection)
    {
        bool crossed = outDirection > 0
            ? previous.X >= faceX && ballPosition.X < faceX
            : previous.X <= faceX && ballPosition.X > faceX;
        if (!crossed)
            return MatchFieldEvents.None;

        float travel = ballPosition.X - previous.X;
        float t = travel == 0 ? 0f : (faceX - previous.X) / travel;
        float crossY = previous.Y + (ballPosition.Y - previous.Y) * t;

        float offset = (crossY - paddleCenter) / PaddleReach;
        if (MathF.Abs(offset) > 1f)
            return MatchFieldEvents.None;

        float slice = Math.Clamp(paddleVelocity / PaddleSpeed, -1f, 1f);
        float angle = ShotAngle(offset, slice);
        float speed = ShotSpeed(ballVelocity.Length, slice);
        ballVelocity = (outDirection * speed * MathF.Cos(angle), speed * MathF.Sin(angle));

        float remaining = (1f - t) * dt;
        ballPosition = (faceX, crossY);
        ballPosition += ballVelocity * remaining;

        return MathF.Abs(slice) >= SliceSoundThreshold ? MatchFieldEvents.PaddleSlice : MatchFieldEvents.PaddleHit;
    }

    /// <summary>Reflects both velocity and overshoot position off the top and bottom field edges.</summary>
    private MatchFieldEvents ReflectOffFieldEdges()
    {
        var events = MatchFieldEvents.None;
        float half = BallSize / 2;

        if (ballPosition.Y < half && ballVelocity.Y < 0)
        {
            ballPosition = (ballPosition.X, half + (half - ballPosition.Y));
            ballVelocity = (ballVelocity.X, -ballVelocity.Y);
            events |= MatchFieldEvents.WallBounce;
        }

        float bottom = Height - half;
        if (ballPosition.Y > bottom && ballVelocity.Y > 0)
        {
            ballPosition = (ballPosition.X, bottom - (ballPosition.Y - bottom));
            ballVelocity = (ballVelocity.X, -ballVelocity.Y);
            events |= MatchFieldEvents.WallBounce;
        }

        return events;
    }
}
