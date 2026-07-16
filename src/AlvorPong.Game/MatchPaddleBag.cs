namespace AlvorPong.Game;

/// <summary>Read surface for the maintained set of initialized paddles.</summary>
[Match]
public sealed class MatchPaddleBag(MatchPaddleBagMut bag) :
    EntIdxGatedBag<MatchEntComponents.IsPaddle, MatchEntComponents.IsReady>(bag);
