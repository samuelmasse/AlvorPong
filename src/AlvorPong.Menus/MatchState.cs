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
    MatchSimulation simulation,
    MatchEntLifetime ents,
    MatchScore score,
    MatchRenderer renderer,
    MatchAudio audio,
    MatchCursor cursor) : State
{
    private EntMut pauseNode;
    private bool paused;
    private bool continuingToMatchOver;

    public override void Load()
    {
        screen.Title = "AlvorPong - Match";
        audio.StartServe();
    }

    public override void Unload()
    {
        HidePauseMenu(playSound: false);
        cursor.Show();
        if (!continuingToMatchOver)
            ents.Unload();
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
        var scorer = simulation.Step(delta, out var events);
        audio.Update(events);

        if (scorer is not { } side)
            return;

        audio.PointScored();
        score.Add(side);
        if (score.Winner != null)
        {
            audio.MatchOver();
            continuingToMatchOver = true;
            state.Current = scope.New<MatchOverState>();
            return;
        }

        simulation.Reset(towards: side == MatchSide.Left ? MatchSide.Right : MatchSide.Left);
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
