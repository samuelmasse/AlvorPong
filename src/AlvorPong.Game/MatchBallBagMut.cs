namespace AlvorPong.Game;

/// <summary>Mutable registration surface for the ready-ball bag.</summary>
[Match]
public sealed class MatchBallBagMut :
    EntIdxGatedBagMut<MatchEntComponents.IsBall, MatchEntComponents.IsReady>;
