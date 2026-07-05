# Repository Instructions

## Visual Studio Solution

Use the committed `AlvorPong.slnx` for normal build, CI, and committed project
layout changes.

For local Visual Studio work that needs AlvorKit projects visible beside
AlvorPong projects, regenerate the ignored development solution:

```powershell
dotnet run --project ../AlvorKit/scripts/AlvorKit.Script.DevSolution
```

Do not hand-edit or commit `AlvorPong.Dev.slnx`; rerun the generator whenever
`AlvorPong.slnx` or `../AlvorKit/AlvorKit.slnx` changes.
