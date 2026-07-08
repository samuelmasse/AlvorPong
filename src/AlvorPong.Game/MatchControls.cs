namespace AlvorPong.Game;

/// <summary>Reads paddle movement axes from a sampled input snapshot, or hands the right paddle to the AI.</summary>
[Match]
public class MatchControls(MatchInput input, MatchAi ai, MatchConfig config)
{
    public float LeftAxis() =>
        input.LeftAxis;

    public float RightAxis() =>
        config.RightIsAi ? ai.Axis() : input.RightAxis;
}
