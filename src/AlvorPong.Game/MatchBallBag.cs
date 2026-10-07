namespace AlvorPong;

/// <summary>Read surface for the maintained set of initialized balls.</summary>
[Match]
public class MatchBallBag :
    EntIdxGatedBag<MatchEntComponents.IsBall, MatchEntComponents.IsReady>;
