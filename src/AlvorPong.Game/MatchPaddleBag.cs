namespace AlvorPong;

/// <summary>Read surface for the maintained set of initialized paddles.</summary>
[Match]
public class MatchPaddleBag :
    EntIdxGatedBag<MatchEntComponents.IsPaddle, MatchEntComponents.IsReady>;
