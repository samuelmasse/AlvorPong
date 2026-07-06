namespace AlvorPong.Menus;

/// <summary>Shows the match result over the frozen final frame and offers a rematch or the way back to the main menu.</summary>
[Match]
public class MatchOverState(
    RootBackbuffer backbuffer,
    RootKeyboard keyboard,
    RootScripts scripts,
    RootUi ui,
    RootUiScript uiScript,
    AppAudio audio,
    AppGameOverMenu menu,
    AppMatchStart matchStart,
    AppMenuReturn menuReturn,
    AppStyle s,
    MatchConfig config,
    MatchRenderer renderer,
    MatchScore score) : State
{
    private EntMut menuNode;

    public override void Load()
    {
        scripts.Add(uiScript);
        menuNode = Node(ui)
            .SizeRelativeV((1, 1));
        {
            menu.Create(menuNode, new AppGameOverView(config, score));
        }
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
        {
            audio.Play(AppSound.MenuConfirm);
            matchStart.Run(config);
        }
        if (keyboard.IsKeyPressed(Keys.Escape))
        {
            audio.Play(AppSound.MenuConfirm);
            menuReturn.Run();
        }
    }

    public override void Draw() => renderer.Draw();

    public override void Render() => backbuffer.Clear(s.Palette.AppBackground);
}
