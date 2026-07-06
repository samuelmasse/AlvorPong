namespace AlvorPong.Game;

/// <summary>Simulation events emitted by a match step for presentation systems such as audio.</summary>
[Flags]
public enum MatchFieldEvents
{
    None = 0,
    ServeLaunch = 1 << 0,
    WallBounce = 1 << 1,
    PaddleHit = 1 << 2,
    PaddleSlice = 1 << 3,
}
