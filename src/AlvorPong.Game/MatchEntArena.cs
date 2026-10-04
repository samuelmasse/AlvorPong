namespace AlvorPong;

/// <summary>Owns all ECS allocations made for one match.</summary>
[Match]
public class MatchEntArena(MatchEntIdxContextBuilder context) : EntIdxArena(context.Ent)
{
    /// <summary>Invalidates match Ents before releasing the match-owned hooks.</summary>
    public override void Dispose()
    {
        base.Dispose();
        context.Dispose();
    }
}
