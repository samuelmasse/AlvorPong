namespace AlvorPong.Game;

/// <summary>Allocates and fully initializes the simulated Ents in one match.</summary>
[Match]
public sealed class MatchEntSpawner(MatchEntArena arena)
{
    /// <summary>Spawns one paddle on the requested side.</summary>
    public EntPtrIdx SpawnPaddle(MatchSide side)
    {
        float x = side == MatchSide.Left
            ? MatchField.PaddleMargin + MatchField.PaddleWidth / 2f
            : MatchField.Width - MatchField.PaddleMargin - MatchField.PaddleWidth / 2f;

        return arena.Alloc().Mutate()
            .Position((x, MatchField.Height / 2f))
            .Velocity(default)
            .MovementAxis(0f)
            .Side(side)
            .IsPaddle(true)
            .IsReady(true);
    }

    /// <summary>Spawns the match ball at the center of the field.</summary>
    public EntPtrIdx SpawnBall() => arena.Alloc().Mutate()
        .Position((MatchField.Width / 2f, MatchField.Height / 2f))
        .Velocity(default)
        .IsBall(true)
        .IsReady(true);
}
