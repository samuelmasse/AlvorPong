namespace AlvorPong;

/// <summary>Bootstraps the AlvorPong app scope from the engine root scope.</summary>
[Root]
public class RootLoadState(RootScripts scripts, RootState state, RootScope scope) : State
{
    public override void Load()
    {
        var app = scope.Scope<AppScope>();
        app.Add(new BlendUi(scope.Get<RootBlend>(), app.Get<AppGl>()));
        scripts.Add(app.Get<AppAudioScript>());
        state.Current = app.New<AppMenuState>();
    }
}
