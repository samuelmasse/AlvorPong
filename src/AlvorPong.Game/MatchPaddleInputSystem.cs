namespace AlvorPong.Game;

/// <summary>Writes sampled player or AI movement intent into paddle components.</summary>
[Match]
public class MatchPaddleInputSystem(
    MatchConfig config,
    MatchInput input,
    MatchAi ai,
    MatchPaddleBag paddles)
{
    /// <summary>Writes the current player or AI axis to each ready paddle.</summary>
    public void Update()
    {
        var ents = paddles.Ents;
        for (var i = 0; i < ents.Length; i++)
        {
            var paddle = ents[i];
            paddle.MovementAxis = paddle.Side switch
            {
                MatchSide.Left => input.LeftAxis,
                _ when config.RightIsAi => ai.Axis(),
                _ => input.RightAxis,
            };
        }
    }
}
