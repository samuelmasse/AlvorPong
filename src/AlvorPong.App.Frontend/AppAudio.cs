namespace AlvorPong.App.Frontend;

/// <summary>Owns the app-wide MiniAudio engine and a fixed pool of preloaded one-shot sounds.</summary>
[App]
public sealed unsafe class AppAudio(Ma ma) : IDisposable
{
    private const int VoicesPerSound = 4;
    private const int SoundCount = (int)AppSound.MatchLoss + 1;

    private readonly MaSound*[] sounds = new MaSound*[SoundCount * VoicesPerSound];
    private readonly int[] nextVoices = new int[SoundCount];
    private MaEngine* engine;
    private bool enabled;
    private bool disposed;

    public void Load()
    {
        if (enabled || disposed)
            return;

        engine = AllocateNativeObject<MaEngine>();
        if (ma.EngineInit(null, engine) != MaResult.Success)
        {
            NativeMemory.Free(engine);
            engine = null;
            return;
        }

        enabled = true;
        LoadSounds();
    }

    public void Play(AppSound sound)
    {
        if (!enabled || disposed)
            return;

        var soundIndex = (int)sound;
        if ((uint)soundIndex >= SoundCount)
            return;

        var next = nextVoices[soundIndex];
        nextVoices[soundIndex] = (next + 1) % VoicesPerSound;

        var voice = sounds[soundIndex * VoicesPerSound + next];
        if (voice == null)
            return;

        _ = ma.SoundStop(voice);
        _ = ma.SoundSeekToPcmFrame(voice, 0);
        _ = ma.SoundStart(voice);
    }

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;
        for (var i = 0; i < sounds.Length; i++)
            UnloadSound(ref sounds[i]);

        if (engine != null)
        {
            ma.EngineUninit(engine);
            NativeMemory.Free(engine);
            engine = null;
        }

        enabled = false;
    }

    private void LoadSounds()
    {
        var root = Path.Combine(AppContext.BaseDirectory, "Audio");
        for (var sound = 0; sound < SoundCount; sound++)
        {
            var soundId = (AppSound)sound;
            var path = Path.Combine(root, $"{soundId}.wav");
            if (!File.Exists(path))
                continue;

            for (var voice = 0; voice < VoicesPerSound; voice++)
                LoadSound(sound * VoicesPerSound + voice, path, Volume(soundId));
        }
    }

    private void LoadSound(int slot, string path, float volume)
    {
        var sound = AllocateNativeObject<MaSound>();
        const MaSoundFlags flags = MaSoundFlags.SoundFlagNoSpatialization;

        if (ma.SoundInitFromFile(engine, path, (uint)flags, null, null, sound) != MaResult.Success)
        {
            NativeMemory.Free(sound);
            return;
        }

        ma.SoundSetVolume(sound, volume);
        sounds[slot] = sound;
    }

    private void UnloadSound(ref MaSound* sound)
    {
        if (sound == null)
            return;

        _ = ma.SoundStop(sound);
        ma.SoundUninit(sound);
        NativeMemory.Free(sound);
        sound = null;
    }

    private static float Volume(AppSound sound) => sound switch
    {
        AppSound.MenuMove => 0.35f,
        AppSound.CountdownTick => 0.45f,
        AppSound.WallBounce => 0.55f,
        AppSound.PaddleHit => 0.65f,
        AppSound.PaddleSlice => 0.7f,
        AppSound.PointScored => 0.75f,
        AppSound.MatchWin => 0.78f,
        AppSound.MatchLoss => 0.72f,
        _ => 0.6f,
    };

    private static T* AllocateNativeObject<T>()
        where T : unmanaged =>
        (T*)NativeMemory.AllocZeroed((nuint)sizeof(T));
}
