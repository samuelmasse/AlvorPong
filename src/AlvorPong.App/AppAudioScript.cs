namespace AlvorPong.App;

/// <summary>Loads app audio at startup and releases MiniAudio state during root shutdown.</summary>
[App]
public class AppAudioScript(AppAudio audio) : Script
{
    public override void Load() => audio.Load();

    public override void Unload() => audio.Dispose();
}
