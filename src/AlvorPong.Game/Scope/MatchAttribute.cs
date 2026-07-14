namespace AlvorPong.Game;

/// <summary>Marks services that belong to one running Pong match.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public class MatchAttribute : InjectorAttribute;
