namespace AlvorPong.Menus;

/// <summary>Props for the game-over menu: the finished match's config for rematches and its final score.</summary>
public record AppGameOverView(MatchConfig Config, MatchScore Score);
