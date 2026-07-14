namespace AlvorPong.App.Frontend;

/// <summary>Owns OpenGL objects that live for the AlvorPong application lifetime.</summary>
[App]
public class AppGl(RootGl gl) : GlLayer(gl);
