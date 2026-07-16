namespace AlvorPong.Game;

/// <summary>Advances the ball, resolves swept paddle and wall collisions, and reports goals.</summary>
[Match]
public class MatchBallSystem(
    MatchField field,
    MatchServe serve,
    MatchPaddleBag paddles,
    MatchBallBag balls)
{
    private const float SliceSoundThreshold = 0.45f;

    /// <summary>Recenters the ball and begins a serve toward the requested side.</summary>
    public void Reset(MatchSide towards)
    {
        var ball = balls.Ents[0];
        ball.Position = (MatchField.Width / 2f, MatchField.Height / 2f);
        ball.Velocity = default;
        serve.Reset(towards);
    }

    /// <summary>Advances ball simulation and returns the scoring side, if any.</summary>
    public MatchSide? Update(double delta, out MatchFieldEvents events)
    {
        events = MatchFieldEvents.None;
        var ball = balls.Ents[0];

        if (serve.IsServing)
        {
            if (serve.Update(delta, out var launchVelocity))
            {
                ball.Velocity = launchVelocity;
                events |= MatchFieldEvents.ServeLaunch;
            }
            return null;
        }

        float dt = (float)delta;
        var previous = ball.Position;
        ball.Position = previous + ball.Velocity * dt;

        events |= BounceOffPaddle(ball, previous, dt);
        events |= ReflectOffFieldEdges(ball);

        if (ball.Position.X < -MatchField.BallSize)
            return MatchSide.Right;
        if (ball.Position.X > MatchField.Width + MatchField.BallSize)
            return MatchSide.Left;
        return null;
    }

    private MatchFieldEvents BounceOffPaddle(EntMutIdx ball, Vec2 previous, float dt)
    {
        var velocity = ball.Velocity;
        if (velocity.X == 0)
            return MatchFieldEvents.None;

        var side = velocity.X < 0 ? MatchSide.Left : MatchSide.Right;
        var ents = paddles.Ents;
        var paddle = ents[0].Side == side ? ents[0] : ents[1];
        float faceX = side == MatchSide.Left ? field.LeftContactX : field.RightContactX;
        float outDirection = side == MatchSide.Left ? 1f : -1f;
        return TrySweptBounce(ball, paddle, previous, dt, faceX, outDirection);
    }

    private MatchFieldEvents TrySweptBounce(
        EntMutIdx ball,
        EntMutIdx paddle,
        Vec2 previous,
        float dt,
        float faceX,
        float outDirection)
    {
        var position = ball.Position;
        bool crossed = outDirection > 0
            ? previous.X >= faceX && position.X < faceX
            : previous.X <= faceX && position.X > faceX;
        if (!crossed)
            return MatchFieldEvents.None;

        float travel = position.X - previous.X;
        float t = travel == 0 ? 0f : (faceX - previous.X) / travel;
        float crossY = previous.Y + (position.Y - previous.Y) * t;

        float offset = (crossY - paddle.Position.Y) / MatchField.PaddleReach;
        if (MathF.Abs(offset) > 1f)
            return MatchFieldEvents.None;

        float slice = Math.Clamp(paddle.Velocity.Y / MatchField.PaddleSpeed, -1f, 1f);
        float angle = field.ShotAngle(offset, slice);
        float speed = field.ShotSpeed(ball.Velocity.Length, slice);
        Vec2 velocity = (outDirection * speed * MathF.Cos(angle), speed * MathF.Sin(angle));

        float remaining = (1f - t) * dt;
        ball.Velocity = velocity;
        ball.Position = (faceX, crossY) + velocity * remaining;

        return MathF.Abs(slice) >= SliceSoundThreshold ? MatchFieldEvents.PaddleSlice : MatchFieldEvents.PaddleHit;
    }

    private MatchFieldEvents ReflectOffFieldEdges(EntMutIdx ball)
    {
        var position = ball.Position;
        var velocity = ball.Velocity;
        var events = MatchFieldEvents.None;
        float half = MatchField.BallSize / 2f;

        if (position.Y < half && velocity.Y < 0)
        {
            position = (position.X, half + (half - position.Y));
            velocity = (velocity.X, -velocity.Y);
            events |= MatchFieldEvents.WallBounce;
        }

        float bottom = MatchField.Height - half;
        if (position.Y > bottom && velocity.Y > 0)
        {
            position = (position.X, bottom - (position.Y - bottom));
            velocity = (velocity.X, -velocity.Y);
            events |= MatchFieldEvents.WallBounce;
        }

        if (events != MatchFieldEvents.None)
        {
            ball.Position = position;
            ball.Velocity = velocity;
        }

        return events;
    }
}
