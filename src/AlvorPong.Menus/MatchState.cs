namespace AlvorPong.Menus;

/// <summary>Runs one Pong match: input, simulation steps, scoring, the pause modal, and cursor auto-hide.</summary>
[Match]
public class MatchState(
    RootBackbuffer backbuffer,
    RootInput input,
    RootKeyboard keyboard,
    RootMouse mouse,
    RootScreen screen,
    RootScripts scripts,
    RootState state,
    RootUi ui,
    RootUiScript uiScript,
    AppPauseMenu pauseMenu,
    AppStyle s,
    MatchScope scope,
    MatchConfig config,
    MatchControls controls,
    MatchField field,
    MatchScore score,
    MatchRenderer renderer) : State
{
    private const double CursorHideDelay = 1.0;

    private EntMut pauseNode;
    private bool paused;
    private bool cursorHidden;
    private double cursorIdleSeconds;
    private Vec2 cursorPosition;

    public override void Load() => screen.Title = "AlvorPong - Match";

    public override void Unload()
    {
        HidePauseMenu();
        ShowCursor();
    }

    public override void Update(double delta)
    {
        if (keyboard.IsKeyPressed(Keys.Escape))
        {
            if (paused)
                HidePauseMenu();
            else
                ShowPauseMenu();
        }

        UpdateCursor(delta);

        if (paused)
            return;

        if (field.Step(delta, controls.LeftAxis(), controls.RightAxis()) is not { } scorer)
            return;

        score.Add(scorer);
        if (score.Winner != null)
            state.Current = scope.New<MatchOverState>();
        else
            field.Reset(towards: scorer == MatchSide.Left ? MatchSide.Right : MatchSide.Left);
    }

    public override void Draw() => renderer.Draw();

    public override void Render() => backbuffer.Clear(s.Palette.AppBackground);

    private void ShowPauseMenu()
    {
        paused = true;
        scripts.Add(uiScript);
        pauseNode = Node(ui)
            .SizeRelativeV((1, 1));
        {
            pauseMenu.Create(pauseNode, new AppPauseView(config, score, HidePauseMenu));
        }
    }

    private void HidePauseMenu()
    {
        if (!paused)
            return;

        paused = false;
        NodesRemove(ui, pauseNode);
        pauseNode = default;
        scripts.Remove(uiScript);
    }

    /// <summary>Hides the cursor after a second without motion during play; any motion or the pause menu shows it again.</summary>
    private void UpdateCursor(double delta)
    {
        if (paused || mouse.Position != cursorPosition)
        {
            cursorPosition = mouse.Position;
            cursorIdleSeconds = 0;
            ShowCursor();
            return;
        }

        cursorIdleSeconds += delta;
        if (cursorIdleSeconds >= CursorHideDelay)
            HideCursor();
    }

    private void ShowCursor()
    {
        if (!cursorHidden)
            return;

        cursorHidden = false;
        input.CursorMode = CursorMode.Normal;
    }

    private void HideCursor()
    {
        if (cursorHidden)
            return;

        cursorHidden = true;
        input.CursorMode = CursorMode.Hidden;
    }
}
