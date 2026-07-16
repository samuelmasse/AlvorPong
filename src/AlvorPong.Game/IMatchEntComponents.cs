namespace AlvorPong.Game;

/// <summary>Declares mutable state shared by the simulated Ents in one Pong match.</summary>
[Components]
public interface IMatchEntComponents
{
    /// <summary>Gets or sets the Ent center in logical field coordinates.</summary>
    [ComponentToString] Vec2 Position { get; set; }

    /// <summary>Gets or sets the Ent velocity in logical field units per second.</summary>
    [ComponentToString] Vec2 Velocity { get; set; }

    /// <summary>Gets or sets the movement intent consumed by paddle movement.</summary>
    float MovementAxis { get; set; }

    /// <summary>Gets or sets the field side owned by a paddle.</summary>
    [ComponentToString] MatchSide Side { get; set; }

    /// <summary>Gets or sets whether the Ent is a paddle.</summary>
    bool IsPaddle { get; set; }

    /// <summary>Gets or sets whether the Ent is the ball.</summary>
    bool IsBall { get; set; }

    /// <summary>Gets or sets whether initialization is complete and systems may observe the Ent.</summary>
    bool IsReady { get; set; }
}
