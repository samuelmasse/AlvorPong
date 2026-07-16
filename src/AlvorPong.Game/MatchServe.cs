namespace AlvorPong.Game;

/// <summary>Tracks the match-wide countdown and direction for the current serve.</summary>
[Match]
public class MatchServe(MatchRandom random)
{
    private const float Speed = 700f;
    private const float Angle = 0.45f;
    private const double Delay = 3.0;

    private double countdown = Delay;
    private MatchSide towards = random.NextSide();

    /// <summary>Gets whether the ball is waiting on the serve countdown.</summary>
    public bool IsServing => countdown > 0;

    /// <summary>Gets the whole seconds left before the serve launches.</summary>
    public int SecondsLeft => (int)Math.Ceiling(countdown);

    /// <summary>Begins a new centered serve toward the requested side.</summary>
    public void Reset(MatchSide side)
    {
        towards = side;
        countdown = Delay;
    }

    /// <summary>Advances the countdown and returns the launch velocity when it completes.</summary>
    public bool Update(double delta, out Vec2 velocity)
    {
        countdown -= delta;
        if (countdown > 0)
        {
            velocity = default;
            return false;
        }

        float angle = random.NextSignedSingle() * Angle;
        float direction = towards == MatchSide.Left ? -1f : 1f;
        velocity = (direction * Speed * MathF.Cos(angle), Speed * MathF.Sin(angle));
        return true;
    }
}
