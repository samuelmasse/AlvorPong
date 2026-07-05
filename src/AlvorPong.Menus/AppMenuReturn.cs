namespace AlvorPong.Menus;

/// <summary>Leaves the current state and returns to the main menu.</summary>
[App]
public class AppMenuReturn(RootState state, AppScope scope)
{
    public void Run() => state.Current = scope.New<AppMenuState>();
}
