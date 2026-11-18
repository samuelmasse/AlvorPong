namespace AlvorPong;

/// <summary>Builds Blend controls with textures owned by the application graphics layer.</summary>
[App]
public class AppBlendUi(RootUi ui, RootInter fonts, AppGl gl) : BlendUi(ui, fonts, gl);
