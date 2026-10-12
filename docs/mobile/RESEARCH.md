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

## Android 16 KB native-library finding

- MonoGame Android `3.8.2.1105` emitted XA0141 for its bundled OpenAL native library.
- MonoGame `3.8.4.1` is the selected probe dependency. Its release notes identify the release as the Android 16 KB policy update and the build no longer emits XA0141.
- The general remediation for custom/prebuilt native libraries is to rebuild every `.so` with 16 KB ELF alignment (`-Wl,-z,max-page-size=16384`; Android also documents `-Wl,-z,common-page-size=16384` for older NDKs), then verify both ELF segment alignment and APK zip alignment.
- If a dependency still fails, alternatives are: upgrade to a fixed vendor package, rebuild/fork the native dependency with the flags, or replace the dependency. Suppressing XA0141 is not a compatibility fix.
- Sources: Android Developers, “Support 16 KB page sizes”; MonoGame 3.8.4.1 release/migration notes. This probe has not yet been tested on a 16 KB-page Android image.

## Game snapshot inventory

- Source installation was inspected read-only while the human editor/server/client were running.
- Server data: `Server/resources/gamedata.db` (map/game definitions), `playerdata.db` (accounts/characters), and `logging.db`.
- Client cache: `Client and Editor/resources/mapcache.db`; static client resources include tilesets, entities, GUI, sounds, music, and fonts.
- A separate snapshot exists at `Engine-Development/test-data/Game-DevelopmentSnapshot`. Each SQLite database was copied through SQLite's online backup API and passed `PRAGMA integrity_check`; static files were copied separately. `SNAPSHOT_MANIFEST.json` records hashes.
- The snapshot's isolated server config uses port `5401`, disables UPnP and external port checks, and the copied client config targets Android emulator host gateway `10.0.2.2:5401`.
- The original installation remains untouched. A full cross-database point-in-time snapshot was not claimed because the source server remained active; isolated server startup must be verified before any client connection test.
