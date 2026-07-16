namespace AlvorPong.Game;

/// <summary>Retains match Ent ownership and performs maintained individual teardown before arena teardown.</summary>
[Match]
public sealed class MatchEntLifetime(MatchEntArena arena, MatchEntSpawner spawner)
{
    private EntPtrIdx leftPaddle;
    private EntPtrIdx rightPaddle;
    private EntPtrIdx ball;

    /// <summary>Creates the complete Ent set after Indexed bags have been registered.</summary>
    public void Load()
    {
        leftPaddle = spawner.SpawnPaddle(MatchSide.Left);
        rightPaddle = spawner.SpawnPaddle(MatchSide.Right);
        ball = spawner.SpawnBall();
    }

    /// <summary>Removes every Ent through Indexed disposal, then releases the arena.</summary>
    public void Unload()
    {
        ball.Dispose();
        rightPaddle.Dispose();
        leftPaddle.Dispose();
        arena.Dispose();
    }
}
