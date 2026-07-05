namespace AlvorPong.Menus;

/// <summary>Shows the main menu and starts matches from it.</summary>
[App]
public class AppMenuState(
    RootBackbuffer backbuffer,
    RootKeyboard keyboard,
    RootScreen screen,
    RootScripts scripts,
    RootUi ui,
    RootUiScript uiScript,
    AppMainMenu menu,
    AppMatchStart matchStart,
    AppStyle s) : State
{
    private EntMut menuNode;

    public override void Load()
    {
        screen.Title = "AlvorPong";
        screen.IsVSyncEnabled = true;
        scripts.Add(uiScript);
        menuNode = Node(ui)
            .SizeRelativeV((1, 1));
        {
            menu.Create(menuNode);
        }
        screen.IsVisible = true;
    }

    public override void Unload()
    {
        if (menuNode != default)
            NodesRemove(ui, menuNode);
        scripts.Remove(uiScript);
    }

    public override void Update(double delta)
    {
        if (keyboard.IsKeyPressed(Keys.Enter))
            matchStart.Run(new MatchConfig(RightIsAi: true));
        if (keyboard.IsKeyPressed(Keys.T))
            matchStart.Run(new MatchConfig(RightIsAi: false));
        if (keyboard.IsKeyPressed(Keys.Escape))
            screen.Close();
    }

    public override void Render() => backbuffer.Clear(s.Palette.AppBackground);
}
