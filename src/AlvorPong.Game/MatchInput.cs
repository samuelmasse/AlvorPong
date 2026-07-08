namespace AlvorPong.Game;

/// <summary>Frame input sampled by the menu/composition layer and consumed by pure match logic.</summary>
[Match]
public class MatchInput
{
    /// <summary>Gets left paddle movement intent, normally -1, 0, or 1.</summary>
    public float LeftAxis { get; private set; }

    /// <summary>Gets right paddle movement intent, normally -1, 0, or 1.</summary>
    public float RightAxis { get; private set; }

    /// <summary>Updates the current frame input snapshot.</summary>
    public void Set(float leftAxis, float rightAxis)
    {
        LeftAxis = leftAxis;
        RightAxis = rightAxis;
    }

    /// <summary>Clears input while menus or overlays own the frame.</summary>
    public void Clear() => Set(0f, 0f);
}
