# Test results

Environment: Windows 11, x64; .NET SDK `10.0.401`; isolated .NET 8 runtime under `Engine-Development/.dotnet8`; Android workload `36.1.2/10.0.100` installed; Git `2.53.0.windows.3`.

## Source baseline

- `git submodule update --init --recursive`: passed; LiteNetLib and dockpanelsuite checked out.
- `dotnet restore Intersect.sln -p:Configuration=Debug`: passed with existing warnings, including package compatibility/vulnerability advisories.
- `dotnet build Intersect.sln -p:Configuration=Debug --no-restore`: passed, 0 errors, 2186 warnings.
- Build emitted existing compiler/package warnings; no source changes preceded this baseline build.

## Tests

- Initial solution/project test invocation returned no test output because test project assets were not restored.
- Targeted `Intersect.Tests.Client` restore/build completed, but test execution aborted: the test host requires `Microsoft.NETCore.App 8.0.0`; only `10.0.12` is installed.
- With `DOTNET_ROOT=C:/Studio/Intersect/Engine-Development/.dotnet8`, verified NUnit results:
  - `Intersect.Tests`: 197 passed, 12 skipped, 0 failed, 209 total.
  - `Intersect.Tests.Client`: 13 passed, 0 skipped, 0 failed, 13 total.
  - `Intersect.Tests.Client.Framework`: 1 passed, 0 skipped, 0 failed, 1 total.
  - `Intersect.Tests.Server`: 44 passed, 0 skipped, 0 failed, 44 total.
  - `Intersect.Tests.Editor` and `Intersect.Tests.Network` exited 0 without a test-run summary (no executable tests reported).

## Android toolchain probe

- `dotnet workload install android --skip-manifest-update`: passed; Android workload `36.1.2/10.0.100` installed for the .NET 10 SDK.
- A minimal `net10.0-android` project using `MonoGame.Framework.Android` `3.8.4.1` restored successfully after constraining runtime identifiers to Android ABIs.
- Local Android SDK and JDK dependencies were provisioned under Engine-Development (`.android-sdk` and `.jdk`).
- `dotnet build experiments/AndroidPoc/Intersect.Android.Poc.csproj -c Debug --no-restore -p:AndroidSdkDirectory=... -p:JavaSdkDirectory=...`: passed, 0 errors, 2 warnings.
- The earlier `XA0141` warning was observed with MonoGame 3.8.2.1105. The 3.8.4.1 build completed without `XA0141`; MonoGame's 3.8.4.1 release notes state that its OpenAL update addresses the Android 16 KB policy. Final APK ELF alignment should still be checked before store distribution.

## Android launch milestone

- Published installable APK: `experiments/AndroidPoc/bin/Release/net10.0-android/android-x64/com.companyname.Intersect.Android.Poc-Signed.apk`.
- Installed on AVD `intersect-poc-api35` (`emulator-5554`) with `adb install -r`: `Success`.
- Launched explicit activity `crc641f51c5f7cf8b1497.MainActivity`; process remained alive (`pid=4607`).
- `dumpsys gfxinfo` reported 2 rendered frames. ADB screenshot was visually verified: full CornflowerBlue surface with a centered dark-green rectangle, no Android error screen.
- The probe now references `Intersect.Client.Framework` and `Intersect.Client.Framework.Core` transitively; the Android build succeeds with that project reference. Client framework runtime behavior is not yet integrated into the render loop.
- The probe also references `Intersect.Client.Core` and `Intersect.Network`; Release publish and emulator launch still succeed. This verifies packaging/initialization compatibility only, not the full client startup path.

## Isolated server snapshot

- Initial isolated-server startup failure was diagnosed from its copied log: the copied `appsettings.Production.json` still bound the REST API to `127.0.0.1:5400`, which belongs to the human installation.
- The isolated appsettings was changed only in `test-data/Game-DevelopmentSnapshot` to bind HTTP API `127.0.0.1:5402`; the game UDP port remains `5401`.
- With a PTY and the isolated .NET 8 host, the server started successfully. Verified listeners: UDP `0.0.0.0:5401` and TCP `127.0.0.1:5402`.
- `http://127.0.0.1:5402/` returned HTTP 200 with the isolated server web page.
- The original server remains on port `5400`; no original files or processes were changed.

## Git credential-helper diagnosis

- `GIT_TERMINAL_PROMPT=0 GCM_INTERACTIVE=Never git ls-remote origin HEAD`: passed.
- Same noninteractive `git push --dry-run origin feature/android-port`: passed, `Everything up-to-date`.
- `git credential-manager diagnose`: 7 checks passed, 0 failed.
- No credential reset, deletion, replacement, force push, or local commit modification was performed. The prior interactive push hang is not reproduced by these read-only/noninteractive checks.

## Stable package smoke check

`Game-Development` contains the v0.8.0-beta.581 Windows full package with separate Client and Editor and Server directories. The packaged client and server started and exited cleanly when invoked with `--help`; the editor process started and returned exit code 1 in the headless smoke probe after logging its startup/version, so interactive editor operation still needs a GUI validation pass.

## Isolation checks

- Engine source/build path: `C:\Studio\Intersect\Engine-Development`
- Stable game path: `C:\Studio\Intersect\Game-Development`
- Game-Development is not a Git worktree and has no source/build linkage to Engine-Development.
- No mobile source changes or experimental binaries were copied into Game-Development.
