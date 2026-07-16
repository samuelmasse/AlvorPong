namespace AlvorPong.Game;

/// <summary>Marks setup services used while one match scope is loading.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public class MatchLoaderAttribute : InjectorAttribute;
