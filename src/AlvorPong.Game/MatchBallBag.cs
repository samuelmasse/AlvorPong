namespace AlvorPong.Game;

/// <summary>Read surface for the maintained set of initialized balls.</summary>
[Match]
public sealed class MatchBallBag(MatchBallBagMut bag) :
    EntIdxGatedBag<MatchEntComponents.IsBall, MatchEntComponents.IsReady>(bag);
