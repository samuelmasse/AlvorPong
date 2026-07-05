# AlvorPong

AlvorPong is a small Pong game built on [AlvorKit](../AlvorKit) as a **sibling-repo pilot**: it
validates that an external game can consume AlvorKit source projects through relative
ProjectReferences before the larger Craftdig migration does the same.

## Setup

Clone AlvorKit next to this repository (same parent directory):

```
Repos/
  AlvorKit/
  AlvorPong/
```

Then build and run:

```
dotnet run --project src/AlvorPong
```

A fresh AlvorKit clone works without any codegen step — its pinned generated binding packages
restore from nuget.org, and native DLLs flow into AlvorPong's output through the NuGet runtimes
targets. If AlvorKit's `out/bindgen` or `out/mathgen` exist, those local projects are used instead
automatically.

## Game

- Main menu: play against the AI, play two players, toggle fullscreen, quit.
- Every serve starts with a 3-2-1 countdown at the center of the field.
- First to 5 points wins; game-over screen offers rematch or main menu.
- Controls: left paddle `W`/`S`, right paddle `I`/`K` (arrow keys also mapped). `Escape` opens the
  pause modal (resume, rematch, main menu, quit).
- Ball flight uses swept collision against the paddle faces, so fast balls cannot tunnel, and
  bounces are position-corrected at walls and paddles.

## Projects

| Project           | Purpose                                                                        |
| ----------------- | ------------------------------------------------------------------------------ |
| `AlvorPong`       | Executable entry point and root boot state                                     |
| `AlvorPong.App`   | App scope and Blend-based style (`AppStyle`)                                   |
| `AlvorPong.Game`  | Match simulation: field, physics, controls, sprite renderer (no UI dependency) |
| `AlvorPong.Menus` | Game states and AlvorKit.UI menus (main menu, pause, game over)                |

## CI

`.github/workflows/ci.yml` checks out AlvorPong and AlvorKit side by side (the same sibling layout
as local development), restores and builds the solution, then runs AlvorKit's lint coordinator
(`AlvorKit.Script.Lint`) against this repository: dotnet format (whitespace and style per the
shared `.editorconfig`), Prettier for Markdown/YAML/JSON, editorconfig-checker, and actionlint for
the workflow files. Because AlvorKit is a private repository, the workflow needs an
`ALVORKIT_TOKEN` repository secret holding a token with read access to `samuelmasse/AlvorKit`.

Run the same lint locally from the repo root:

```
dotnet run --project ../AlvorKit/scripts/AlvorKit.Script.Lint -- --repo-root .
```

## Sibling-setup findings (validated 2026-07-05)

Verified end to end with an AlvorSense session: menu clicks, match input, AI, pause, game over,
rematch, and return to menu all work when AlvorKit is consumed purely through sibling
ProjectReferences.

- Blend's Inter fonts are embedded in `AlvorKit.UI.Blend` (`RootInter`, mirroring `RootRoboto`), so
  this repo ships no font assets at all — `AppStyle` is a one-line `BlendStyle` subclass.
- `ProjectRoot.ResDirectory` (AlvorKit.Script.Workspace) hardcodes `AlvorKit.slnx` as its root
  marker, so external repos cannot use it for their own `res/`; a consumer with real assets should
  copy them to the output directory and load via `AppContext.BaseDirectory`.
- `InjectorScope.With(instance)` validates that the seeded instance's type carries the target
  scope's attribute — seed values like `MatchConfig` must be marked `[Match]`.
- Native DLLs (GLFW, FreeType) flow into this repo's output transitively through the cross-repo
  ProjectReferences; no manual copying.
- UI text layout settles on the first update, not the first render — automation that reads element
  positions from a frame-0 screenshot will click stale coordinates.
- Everything else resolves cleanly across repos; see `src/Directory.Build.props` for the
  `$(AlvorKitRoot)` wiring and the sibling-clone existence check.
