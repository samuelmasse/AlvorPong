namespace AlvorPong.Menus;

/// <summary>Builds full-width Blend menu buttons with an optional display-only key chip.</summary>
[App]
public class AppMenuButton(AppAudio audio, AppStyle s)
{
    private const float KeyChipInset = 4f;

    public EntMut Create(EntMut parent, string text, string? key, bool primary, Action onClick)
    {
        Node(parent, out var button)
            .Mutate(primary ? s.ActiveButton : s.Button)
            .SizeTextRelativeV((0, 0))
            .SizeRelativeV((1, 0))
            .TextV(text)
            .OnClickF(() =>
            {
                audio.Play(AppSound.MenuConfirm);
                onClick();
            });

        if (key != null)
        {
            Node(button)
                .Mutate(s.KeyChip)
                .IsFloatingV(true)
                .AlignmentV(Alignment.Right | Alignment.Vertical)
                .OffsetV((-KeyChipInset, 0))
                .TextV(key);
        }

        return button;
    }
}
