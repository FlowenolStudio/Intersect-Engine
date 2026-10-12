# Test results

Environment: Windows 11, x64; .NET SDK `10.0.401`; no .NET 8 runtime installed; Android workload `36.1.2/10.0.100` installed; Git `2.53.0.windows.3`.

## Source baseline

- `git submodule update --init --recursive`: passed; LiteNetLib and dockpanelsuite checked out.
- `dotnet restore Intersect.sln -p:Configuration=Debug`: passed with existing warnings, including package compatibility/vulnerability advisories.
- `dotnet build Intersect.sln -p:Configuration=Debug --no-restore`: passed, 0 errors, 2186 warnings.
- Build emitted existing compiler/package warnings; no source changes preceded this baseline build.

## Tests

- Initial solution/project test invocation returned no test output because test project assets were not restored.
- Targeted `Intersect.Tests.Client` restore/build completed, but test execution aborted: the test host requires `Microsoft.NETCore.App 8.0.0`; only `10.0.12` is installed.
- Result: tests are **not verified** in this environment. No successful test count is claimed.

## Android toolchain probe

- `dotnet workload install android --skip-manifest-update`: passed; Android workload `36.1.2/10.0.100` installed for the .NET 10 SDK.
- A minimal `net10.0-android` project using `MonoGame.Framework.Android` `3.8.2.1105` restored successfully after constraining runtime identifiers to Android ABIs.
- Local Android SDK and JDK dependencies were provisioned under Engine-Development (`.android-sdk` and `.jdk`).
- `dotnet build experiments/AndroidPoc/Intersect.Android.Poc.csproj -c Debug --no-restore -p:AndroidSdkDirectory=... -p:JavaSdkDirectory=...`: passed, 0 errors, 2 warnings.
- The warning is `XA0141`: MonoGame 3.8.2.1105 bundles `libopenal32.so` without Android 16's 16 KB page-size compatibility. No APK/device launch was attempted; `adb` is not on PATH.

## Stable package smoke check

`Game-Development` contains the v0.8.0-beta.581 Windows full package with separate Client and Editor and Server directories. The packaged client and server started and exited cleanly when invoked with `--help`; the editor process started and returned exit code 1 in the headless smoke probe after logging its startup/version, so interactive editor operation still needs a GUI validation pass.

## Isolation checks

- Engine source/build path: `C:\Studio\Intersect\Engine-Development`
- Stable game path: `C:\Studio\Intersect\Game-Development`
- Game-Development is not a Git worktree and has no source/build linkage to Engine-Development.
- No mobile source changes or experimental binaries were copied into Game-Development.
