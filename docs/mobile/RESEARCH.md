# Android compatibility research

## Baseline

- Repository: `https://github.com/FlowenolStudio/Intersect-Engine`
- Upstream: `https://github.com/AscensionGameDev/Intersect-Engine`
- Baseline tag: `v0.8.0-beta.581`
- Baseline commit: `47bccc0070c62259ec70e7f102033d4c8e263299`
- Source baseline was built before any engine source changes.

## Verified architecture

- Client executable: `Intersect.Client` plus `Intersect.Client.Core`.
- Shared client framework: `Intersect.Client.Framework`.
- Networking: `Intersect.Network`, using the LiteNetLib submodule and UDP-oriented connection interfaces.
- Server: `Intersect.Server` and `Intersect.Server.Core`.
- Editor: `Intersect.Editor`, Windows Forms and DirectX-oriented.
- Rendering/input: MonoGame DesktopGL 3.8.2.1105 in client core; current input uses MonoGame keyboard and mouse APIs.
- Native runtime extraction is explicitly desktop-oriented: SDL2/OpenAL assets are embedded for Windows, Linux, and macOS. No Android runtime assets or Android target framework are present in the baseline client project.
- Client startup loads `MonoGame.Framework.Client.dll` from the current working directory and extracts native dependencies. Android asset/resource paths and lifecycle handling will need a platform adapter rather than assuming desktop working-directory semantics.

## Confirmed blockers

1. The client project is not multi-targeted; it inherits `net8.0`, while the editor explicitly targets `net8.0-windows` and uses WinForms/WindowsDX-specific dependencies.
2. Android build tooling/workloads are not installed in the current environment; `dotnet --info` reports no workloads.
3. The current client packages are DesktopGL and SDL/OpenAL desktop native assets, not an Android MonoGame backend.
4. Input is currently keyboard/mouse based; touch input is not implemented in the client input path.
5. Android lifecycle, scoped storage, asset packaging, audio backend, graphics backend, and screen scaling have not yet been implemented or validated.

## Initial approach

Keep the authoritative server and protocol unchanged. First isolate a reusable client-core target behind platform-neutral interfaces, then add an Android-specific host/backend only after a minimal Android build proves the MonoGame version/backend combination. Preserve the existing desktop client project and data directories throughout.
