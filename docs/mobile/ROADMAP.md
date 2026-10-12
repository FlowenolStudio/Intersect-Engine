# Mobile roadmap

1. **Workspace and baseline (complete)** — isolate Engine-Development and Game-Development; record exact baseline; build source.
2. **Platform inventory (complete)** — identify client/core/framework/network/server/editor boundaries and desktop-only dependencies.
3. **Android toolchain proof (next)** — install/use an approved Android-capable .NET/MonoGame toolchain without changing Game-Development.
4. **Minimal Android host** — compile and launch a blank Android host with reusable client-core components.
5. **Rendering/content** — initialize renderer and load existing Intersect content.
6. **Network compatibility** — connect to an unmodified server and authenticate.
7. **Gameplay input** — touch movement and one basic interaction.
8. **Stability/performance** — measure startup, frame time, memory, asset loading, suspend/resume.
9. **Desktop regression gate** — rerun desktop build/tests and verify no Game-Development files or ports were changed.

The editor and server remain desktop/stable; no iOS work begins before Android feasibility is demonstrated.
