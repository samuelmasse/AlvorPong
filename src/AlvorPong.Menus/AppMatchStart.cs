namespace AlvorPong.Menus;

/// <summary>Creates a fresh match scope from the app scope and enters the match state.</summary>
[App]
public class AppMatchStart(RootState state, RootRoboto roboto, AppScope scope)
{
    /// <summary>Creates, loads, and enters a match with the selected rules.</summary>
    public void Run(MatchConfig config) => scope.Scope<MatchScope>()
        .With(config)
        .With(new MatchFonts(roboto.Font))
        .Run(match => match.Scope<MatchLoaderScope>()
            .Run(loader => loader.Get<MatchEntLoader>().Run()))
        .Run(x => state.Current = x.New<MatchState>());
}
