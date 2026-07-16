namespace AlvorPong.Game;

/// <summary>Owns all ECS allocations made for one match.</summary>
[Match]
public sealed class MatchEntArena(MatchEntIdxContextBuilder context) : EntIdxArena(context.Ent);
