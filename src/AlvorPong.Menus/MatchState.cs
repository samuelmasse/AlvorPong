namespace AlvorPong.Menus;

/// <summary>Runs one Pong match: input, simulation steps, scoring, pause, and state transitions.</summary>
[Match]
public class MatchState(
    RootBackbuffer backbuffer,
    RootKeyboard keyboard,
    RootScreen screen,
    RootScripts scripts,
    RootState state,
    RootUi ui,
    RootUiScript uiScript,
    AppPauseMenu pauseMenu,
    AppStyle s,
    MatchScope scope,
    MatchConfig config,
    MatchInput input,
    MatchControls controls,
    MatchField field,
    MatchScore score,
    MatchRenderer renderer,
    MatchAudio audio,
    MatchCursor cursor) : State
{
    private EntMut pauseNode;
    private bool paused;

    public override void Load()
    {
        screen.Title = "AlvorPong - Match";
        audio.StartServe();
    }

    public override void Unload()
    {
        HidePauseMenu(playSound: false);
        cursor.Show();
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

        cursor.Update(delta, paused);

        if (paused)
        {
            input.Clear();
            return;
        }

        input.Set(Axis(Keys.W, Keys.S), Axis(Keys.I, Keys.K) + Axis(Keys.Up, Keys.Down));
        var scorer = field.Step(delta, controls.LeftAxis(), controls.RightAxis(), out var events);
        audio.Update(events);

        if (scorer is not { } side)
            return;

        audio.PointScored();
        score.Add(side);
        if (score.Winner != null)
        {
            audio.MatchOver();
            state.Current = scope.New<MatchOverState>();
            return;
        }

        field.Reset(towards: side == MatchSide.Left ? MatchSide.Right : MatchSide.Left);
        audio.StartServe();
    }

    public override void Draw() => renderer.Draw();

    public override void Render() => backbuffer.Clear(s.Palette.AppBackground);

    private void ShowPauseMenu()
    {
        audio.PauseIn();
        paused = true;
        scripts.Add(uiScript);
        pauseNode = Node(ui)
            .SizeRelativeV((1, 1));
        {
            pauseMenu.Create(pauseNode, new AppPauseView(config, score, () => HidePauseMenu()));
        }
    }

    private void HidePauseMenu(bool playSound = true)
    {
        if (!paused)
            return;

        if (playSound)
            audio.PauseOut();

        paused = false;
        NodesRemove(ui, pauseNode);
        pauseNode = default;
        scripts.Remove(uiScript);
    }

    private float Axis(Keys up, Keys down) =>
        (keyboard.IsKeyDown(down) ? 1f : 0f) - (keyboard.IsKeyDown(up) ? 1f : 0f);
}
