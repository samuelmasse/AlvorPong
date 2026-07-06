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
    AppAudio audio,
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
    private int serveSecondsLeft;
    private Vec2 cursorPosition;

    public override void Load()
    {
        screen.Title = "AlvorPong - Match";
        StartServeCountdownSound();
    }

    public override void Unload()
    {
        HidePauseMenu(playSound: false);
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

        var scorer = field.Step(delta, controls.LeftAxis(), controls.RightAxis(), out var events);
        PlayFieldEvents(events);
        UpdateServeCountdownSound();

        if (scorer is not { } side)
            return;

        audio.Play(AppSound.PointScored);
        score.Add(side);
        if (score.Winner != null)
        {
            audio.Play(MatchOverSound());
            state.Current = scope.New<MatchOverState>();
        }
        else
        {
            field.Reset(towards: side == MatchSide.Left ? MatchSide.Right : MatchSide.Left);
            StartServeCountdownSound();
        }
    }

    public override void Draw() => renderer.Draw();

    public override void Render() => backbuffer.Clear(s.Palette.AppBackground);

    private void ShowPauseMenu()
    {
        audio.Play(AppSound.PauseIn);
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
            audio.Play(AppSound.PauseOut);

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

    private void StartServeCountdownSound()
    {
        if (!field.IsServing)
            return;

        serveSecondsLeft = field.ServeSecondsLeft;
        audio.Play(AppSound.CountdownTick);
    }

    private void UpdateServeCountdownSound()
    {
        if (!field.IsServing)
            return;

        var next = field.ServeSecondsLeft;
        if (next == serveSecondsLeft)
            return;

        serveSecondsLeft = next;
        audio.Play(AppSound.CountdownTick);
    }

    private void PlayFieldEvents(MatchFieldEvents events)
    {
        if ((events & MatchFieldEvents.ServeLaunch) != 0)
            audio.Play(AppSound.ServeLaunch);
        if ((events & MatchFieldEvents.WallBounce) != 0)
            audio.Play(AppSound.WallBounce);
        if ((events & MatchFieldEvents.PaddleSlice) != 0)
            audio.Play(AppSound.PaddleSlice);
        else if ((events & MatchFieldEvents.PaddleHit) != 0)
            audio.Play(AppSound.PaddleHit);
    }

    private AppSound MatchOverSound() =>
        config.RightIsAi && score.Winner == MatchSide.Right ? AppSound.MatchLoss : AppSound.MatchWin;
}
