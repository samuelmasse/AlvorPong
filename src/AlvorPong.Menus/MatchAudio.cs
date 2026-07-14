namespace AlvorPong.Menus;

/// <summary>Translates match simulation and state events into app audio playback.</summary>
[Match]
public class MatchAudio(AppAudio audio, MatchConfig config, MatchField field, MatchScore score)
{
    private int serveSecondsLeft;

    /// <summary>Starts the audible countdown for the current serve.</summary>
    public void StartServe()
    {
        if (!field.IsServing)
            return;

        serveSecondsLeft = field.ServeSecondsLeft;
        audio.Play(AppSound.CountdownTick);
    }

    /// <summary>Plays semantic field events and advances the audible serve countdown.</summary>
    public void Update(MatchFieldEvents events)
    {
        if ((events & MatchFieldEvents.ServeLaunch) != 0)
            audio.Play(AppSound.ServeLaunch);
        if ((events & MatchFieldEvents.WallBounce) != 0)
            audio.Play(AppSound.WallBounce);
        if ((events & MatchFieldEvents.PaddleSlice) != 0)
            audio.Play(AppSound.PaddleSlice);
        else if ((events & MatchFieldEvents.PaddleHit) != 0)
            audio.Play(AppSound.PaddleHit);

        if (!field.IsServing)
            return;

        var next = field.ServeSecondsLeft;
        if (next == serveSecondsLeft)
            return;

        serveSecondsLeft = next;
        audio.Play(AppSound.CountdownTick);
    }

    /// <summary>Plays the point-scored sound.</summary>
    public void PointScored() => audio.Play(AppSound.PointScored);

    /// <summary>Plays the pause-open sound.</summary>
    public void PauseIn() => audio.Play(AppSound.PauseIn);

    /// <summary>Plays the pause-close sound.</summary>
    public void PauseOut() => audio.Play(AppSound.PauseOut);

    /// <summary>Plays the local player's win or loss sound after the match ends.</summary>
    public void MatchOver() =>
        audio.Play(config.RightIsAi && score.Winner == MatchSide.Right ? AppSound.MatchLoss : AppSound.MatchWin);
}
