namespace AlvorPong.Game;

/// <summary>
/// Steers the right paddle: aims each return at the corner farthest from the player and slices through
/// the contact for a bent, faster shot when time allows. Stays beatable through human limits: it reads
/// the ball only past midfield, runs slower than the player, its aim error grows with ball speed, it
/// projects the incoming ball in a straight line (bank shots fool it until the bounce), and it
/// rubber-bands on the score — sloppier and slower with a lead, sharp again when trailing.
/// </summary>
[Match]
public class MatchAi(
    MatchRandom random,
    MatchField field,
    MatchScore score,
    MatchPaddleBag paddles,
    MatchBallBag balls)
{
    private const float ReactionX = MatchField.Width * 0.54f;
    private const float SpeedFactor = 0.84f;
    private const float LeadSpeedDrop = 0.10f;
    private const float TrailSpeedGain = 0.06f;
    private const float AimErrorBase = 16f;
    private const float LeadSloppiness = 14f;
    private const float TrailFocus = 6f;
    private const float AimErrorMax = 24f;
    private const float PressureCaution = 0.25f;
    private const float Deadzone = 6f;
    private const float ApproachBand = 60f;
    private const float SliceWindow = 0.12f;
    private const float OffsetTolerance = 0.15f;
    private const double SliceChance = 0.65;
    private const int OffsetCandidateCount = 7;

    private bool planned;
    private float plannedVySign;
    private float paddleTarget;
    private float sliceDirection;

    /// <summary>Computes the right paddle's movement intent for the current simulation step.</summary>
    public float Axis()
    {
        var ball = balls.Ents[0];
        var ballPosition = ball.Position;
        var ballVelocity = ball.Velocity;
        PaddleYs(out var leftPaddleY, out var rightPaddleY);

        if (ballVelocity.X <= 0f || ballPosition.X < ReactionX)
        {
            planned = false;
            return Approach(MatchField.Height / 2f, rightPaddleY);
        }

        float timeLeft = (field.RightContactX - ballPosition.X) / ballVelocity.X;
        if (timeLeft <= 0f)
            return Approach(ballPosition.Y, rightPaddleY);

        if (planned && MathF.Sign(ballVelocity.Y) != plannedVySign)
            planned = false;

        if (!planned)
            Plan(ballPosition, ballVelocity, leftPaddleY, rightPaddleY, timeLeft);

        if (sliceDirection == 0f)
            return Approach(paddleTarget, rightPaddleY);

        if (timeLeft < SliceWindow)
            return sliceDirection * RunSpeed() / MatchField.PaddleSpeed;

        return Approach(paddleTarget - sliceDirection * RunSpeed() * SliceWindow, rightPaddleY);
    }

    /// <summary>
    /// Picks the contact: where to intercept, which corner to punish, and whether to slice. The read is a
    /// straight-line projection clamped to the field — a bank shot re-plans at its bounce, often too late.
    /// </summary>
    private void Plan(Vec2 ballPosition, Vec2 ballVelocity, float leftPaddleY, float rightPaddleY, float timeLeft)
    {
        planned = true;
        plannedVySign = MathF.Sign(ballVelocity.Y);
        sliceDirection = 0f;

        float straightY = ballPosition.Y + ballVelocity.Y * timeLeft;
        float interceptY = Math.Clamp(straightY, MatchField.BallSize / 2f, MatchField.Height - MatchField.BallSize / 2f);
        float ballSpeed = ballVelocity.Length;
        float pressure = ballSpeed / MatchField.MaxSpeed;

        float bestOffset = 0f;
        float bestDistance = -1f;
        for (var candidateIndex = 0; candidateIndex < OffsetCandidateCount; candidateIndex++)
        {
            var candidate = candidateIndex switch
            {
                0 => -0.85f,
                1 => -0.6f,
                2 => -0.35f,
                3 => 0f,
                4 => 0.35f,
                5 => 0.6f,
                _ => 0.85f,
            };

            if (MathF.Abs(candidate) > 1f - PressureCaution * pressure)
                continue;

            float target = ClampPaddle(interceptY - candidate * MatchField.PaddleReach);
            float achieved = (interceptY - target) / MatchField.PaddleReach;
            if (MathF.Abs(achieved - candidate) > OffsetTolerance)
                continue;
            if (MathF.Abs(target - rightPaddleY) > RunSpeed() * timeLeft)
                continue;

            float distance = MathF.Abs(Landing(interceptY, ballSpeed, achieved) - leftPaddleY);
            if (distance > bestDistance)
            {
                bestDistance = distance;
                bestOffset = achieved;
            }
        }

        paddleTarget = ClampPaddle(interceptY - bestOffset * MatchField.PaddleReach + AimError(pressure));

        float direction = MathF.Sign(bestOffset);
        if (direction == 0f || random.NextDouble() >= SliceChance)
            return;

        float lead = paddleTarget - direction * RunSpeed() * SliceWindow;
        bool leadReachable = ClampPaddle(lead) == lead
            && MathF.Abs(lead - rightPaddleY) <= RunSpeed() * (timeLeft - SliceWindow);
        if (leadReachable)
            sliceDirection = direction;
    }

    /// <summary>Predicts where a placed return at the given contact offset lands on the player's paddle plane.</summary>
    private float Landing(float interceptY, float ballSpeed, float offset)
    {
        float angle = field.ShotAngle(offset, 0f);
        float speed = field.ShotSpeed(ballSpeed, 0f);
        Vec2 velocity = (-speed * MathF.Cos(angle), speed * MathF.Sin(angle));
        return field.PredictBallY((field.RightContactX, interceptY), velocity, field.LeftContactX);
    }

    /// <summary>Gets the score rubber-band: 1 when the AI leads by the full match, -1 when it trails by it.</summary>
    private float Handicap =>
        Math.Clamp((score.Right - score.Left) / (float)MatchScore.WinScore, -1f, 1f);

    /// <summary>Draws the plan's aim error: a wobble on slow balls, a whiffing shank at full speed, wider with a lead.</summary>
    private float AimError(float pressure)
    {
        float sigma = AimErrorBase
            + LeadSloppiness * Math.Max(0f, Handicap)
            - TrailFocus * Math.Max(0f, -Handicap)
            + AimErrorMax * pressure * pressure;
        float gaussian = random.NextSignedSingle() + random.NextSignedSingle() + random.NextSignedSingle();
        return gaussian * sigma;
    }

    private float Approach(float targetY, float paddleY)
    {
        float error = targetY - paddleY;
        if (MathF.Abs(error) < Deadzone)
            return 0f;
        float axisLimit = RunSpeed() / MatchField.PaddleSpeed;
        return Math.Clamp(error / ApproachBand, -axisLimit, axisLimit);
    }

    private float RunSpeed() =>
        MatchField.PaddleSpeed * (SpeedFactor
            - LeadSpeedDrop * Math.Max(0f, Handicap)
            + TrailSpeedGain * Math.Max(0f, -Handicap));

    private void PaddleYs(out float left, out float right)
    {
        var ents = paddles.Ents;
        var first = ents[0];
        var second = ents[1];
        if (first.Side == MatchSide.Left)
        {
            left = first.Position.Y;
            right = second.Position.Y;
        }
        else
        {
            left = second.Position.Y;
            right = first.Position.Y;
        }
    }

    private static float ClampPaddle(float y) =>
        Math.Clamp(y, MatchField.PaddleHeight / 2f, MatchField.Height - MatchField.PaddleHeight / 2f);
}
