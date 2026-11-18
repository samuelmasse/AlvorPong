namespace AlvorPong;

/// <summary>Shows the match result over the frozen final frame and offers a rematch or the way back to the main menu.</summary>
[Match]
public class MatchOverState(
    RootBackbuffer backbuffer,
    RootKeyboard keyboard,
    RootUi ui,
    AppAudio audio,
    AppGameOverMenu menu,
    AppMatchStart matchStart,
    AppMenuReturn menuReturn,
    AppBlendUi bl,
    MatchConfig config,
    MatchRenderer renderer,
    MatchScore score,
    MatchEntLifetime ents) : State
{
    private EntMut menuNode;

    public override void Load()
    {
        menuNode = Node(ui.Default.Root)
            .SizeRelativeV((1, 1));
        {
            menu.Create(menuNode, new AppGameOverView(config, score));
        }
    }

    public override void Unload()
    {
        NodesRemove(ui.Default.Root, menuNode);
        ui.Mouse.Release();
        ents.Unload();
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

    public override void Render() => backbuffer.Clear(bl.S.Palette.AppBackground);
}
