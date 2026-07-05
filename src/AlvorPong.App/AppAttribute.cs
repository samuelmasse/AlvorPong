namespace AlvorPong.App;

/// <summary>Marks services that belong to the AlvorPong application lifetime.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public class AppAttribute : InjectorAttribute;
