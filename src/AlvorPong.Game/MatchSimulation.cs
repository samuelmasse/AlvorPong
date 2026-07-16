namespace AlvorPong.Game;

/// <summary>Runs the focused match systems in simulation order.</summary>
[Match]
public class MatchSimulation(
    MatchPaddleInputSystem paddleInput,
    MatchPaddleMovementSystem paddleMovement,
    MatchBallSystem ball)
{
    /// <summary>Advances input, paddle movement, and ball simulation in dependency order.</summary>
    public MatchSide? Step(double delta, out MatchFieldEvents events)
    {
        paddleInput.Update();
        paddleMovement.Update(delta);
        return ball.Update(delta, out events);
    }

    /// <summary>Recenters the ball and begins a serve toward the requested side.</summary>
    public void Reset(MatchSide towards) => ball.Reset(towards);
}
