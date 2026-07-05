namespace AlvorPong.Game;

/// <summary>Reads paddle movement axes from the keyboard, or hands the right paddle to the AI.</summary>
[Match]
public class MatchControls(RootKeyboard keyboard, MatchAi ai, MatchConfig config)
{
    public float LeftAxis() =>
        Axis(Keys.W, Keys.S);

    public float RightAxis() =>
        config.RightIsAi ? ai.Axis() : Axis(Keys.I, Keys.K) + Axis(Keys.Up, Keys.Down);

    private float Axis(Keys up, Keys down) =>
        (keyboard.IsKeyDown(down) ? 1f : 0f) - (keyboard.IsKeyDown(up) ? 1f : 0f);
}
