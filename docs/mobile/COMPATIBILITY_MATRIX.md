# Compatibility matrix

| Component | Desktop baseline | Android status | Evidence / next step |
|---|---|---|---|
| Core/shared libraries | Builds on .NET 8 | Android-targeted build passes | Continue runtime dependency audit |
| Client core | Builds; MonoGame DesktopGL 3.8.2.1105 | Android-targeted project reference compiles and is packaged; startup not integrated | Replace/abstract desktop backend and native asset packaging |
| Client host | Windows desktop executable | Minimal MonoGame Android Activity launches and renders; Intersect host not integrated | Add Android lifecycle adapter around client context |
| Input | Keyboard/mouse | Not implemented | Add touch-to-action mapping without changing server authority |
| Rendering | DesktopGL/SDL/OpenGL desktop assets | MonoGame Android rendering verified with synthetic probe; real map not loaded | Prove Android graphics backend with existing content |
| Audio | OpenAL desktop resources | Not validated | Verify Android audio backend and lifecycle |
| Networking | LiteNetLib UDP path | Android compile path present; no Android connection verified | Connect to isolated copied-data server |
| Authentication | Existing client/server flow | Unvalidated | Test unchanged protocol after mobile host exists |
| File/settings | Desktop current-directory/resource paths | Blocked for actual client | Map to Android app-private/scoped storage |
| Editor | Windows Forms/Windows target | Not applicable | Keep desktop-only and isolated |
| Server | Desktop/server package | Not applicable | Keep unchanged for compatibility |
| Tests | NUnit projects target .NET 8 | 255 passed, 12 skipped, 0 failed across executable suites | Keep runtime pinned for regression runs |
