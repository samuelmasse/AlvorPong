namespace AlvorPong.Menus;

/// <summary>Props for the pause menu: the running match's config and score, and the resume action.</summary>
public record AppPauseView(MatchConfig Config, MatchScore Score, Action Resume);
