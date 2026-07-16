namespace AlvorPong.Game;

/// <summary>Mutable registration surface for the ready-paddle bag.</summary>
[Match]
public sealed class MatchPaddleBagMut :
    EntIdxGatedBagMut<MatchEntComponents.IsPaddle, MatchEntComponents.IsReady>;
