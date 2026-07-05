namespace AlvorPong.Menus;

/// <summary>Creates a fresh match scope from the app scope and enters the match state.</summary>
[App]
public class AppMatchStart(RootState state, AppScope scope)
{
    public void Run(MatchConfig config) => scope.Scope<MatchScope>()
        .With(config)
        .Run(x => state.Current = x.New<MatchState>());
}
