# Mobile progress

## Completed

- Fork created: `FlowenolStudio/Intersect-Engine`.
- `origin` points to the studio fork; `upstream` points to the official repository.
- Exact baseline verified: `v0.8.0-beta.581` at `47bccc0070c62259ec70e7f102033d4c8e263299`.
- Branches established: `main`, `develop`, and `feature/android-port`; current branch is `feature/android-port`.
- Recursive submodules initialized.
- Unmodified solution restored and built successfully: 0 errors, 2186 warnings.
- Stable Windows package installed separately in `Game-Development`.
- Initial Android compatibility evidence and blockers documented.
- Minimal MonoGame Android APK installed and launched on `intersect-poc-api35`; screenshot verified actual rendering.
- Existing NUnit tests executed with the isolated .NET 8 runtime: 255 passed, 12 skipped, 0 failed across the four executable test projects.
- `Intersect.Client.Framework` is now referenced by the Android probe; compilation succeeds, but client framework runtime integration is not yet complete.
- `Intersect.Client.Core`, `Intersect.Network`, and their shared dependencies now compile for the Android probe; the APK launches with these assemblies packaged, but the real client startup path is not yet called.
- Read-only inspection identified `Server/resources/gamedata.db`, `playerdata.db`, `logging.db`, and `Client and Editor/resources/mapcache.db`; SQLite online backups and static resources were copied to `test-data/Game-DevelopmentSnapshot` with SHA-256 manifest verification.
- Isolated Intersect server now runs from the copied snapshot on UDP `5401`; its REST API is isolated on TCP `5402` and returns HTTP 200.
- The Android host now invokes `Intersect.Client.Core.Program.Main` and reaches the real bootstrap sequence: logger, built-in packet registry, packet-handler registry, plugin factory, client configuration, `ClientContext`, and the Android platform-runner selection.
- AOT/linker seams were added for reflection-heavy startup (command-line options, packet-handler delegates, and the platform runner). The current runtime blocker is inside `IntersectGame` construction immediately after `GraphicsDeviceManager` creation; Android repeatedly recreates the activity before the renderer/preferences phase completes.
- Snapshot verification found six persisted maps in `gamedata.db`, six corresponding PNG previews in `mapcache.db`, a map-folder list containing all six IDs, and fourteen tilesets. `Active Maps: 0` therefore does not mean the copied map data is absent; map synchronization remains unverified because the client has not reached networking/rendering.

## Active

- Android workload and local SDK/JDK dependencies are installed in the Engine development environment; minimal MonoGame Android project builds.
- Diagnose the Android-specific `IntersectGame` construction/activity-recreation blocker without changing the server protocol.
- After the game view is stable, verify the first UDP packet exchange with `10.0.2.2:5401`, then authentication and map synchronization.

## Next actions

1. Validate the interactive stable editor/client/server with the human developer or a GUI-capable smoke run.
2. Inventory the actual MonoGame Android backend options compatible with the pinned client package versions.
3. Create a minimal Android host project without changing the desktop client or server.
4. Validate the packaged APK's native ELF/zip alignment and test against a 16 KB-page Android image if available.
5. Add the Android client network/lifecycle seam and connect it to the isolated UDP server on port 5401; do not use the original port 5400 server.
