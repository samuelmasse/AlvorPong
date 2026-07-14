namespace AlvorPong.Menus;

/// <summary>Applies the match cursor auto-hide policy without owning match state transitions.</summary>
[Match]
public class MatchCursor(RootInput input, RootMouse mouse)
{
    private const double HideDelay = 1.0;

    private Vec2 position;
    private double idleSeconds;
    private bool hidden;

    /// <summary>Updates cursor visibility from motion and pause state.</summary>
    public void Update(double delta, bool paused)
    {
        if (paused || mouse.Position != position)
        {
            position = mouse.Position;
            idleSeconds = 0;
            Show();
            return;
        }

        idleSeconds += delta;
        if (idleSeconds < HideDelay || hidden)
            return;

        hidden = true;
        input.CursorMode = CursorMode.Hidden;
    }

    /// <summary>Restores the normal cursor when leaving the match or opening an overlay.</summary>
    public void Show()
    {
        if (!hidden)
            return;

        hidden = false;
        input.CursorMode = CursorMode.Normal;
    }
}
