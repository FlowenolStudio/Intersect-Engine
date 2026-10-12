# Compatibility matrix

| Component | Desktop baseline | Android status | Evidence / next step |
|---|---|---|---|
| Core/shared libraries | Builds on .NET 8 | Likely reusable, unvalidated | Build a minimal Android-targeted library slice |
| Client core | Builds; MonoGame DesktopGL 3.8.2.1105 | Blocked | Replace/abstract desktop backend and native asset packaging |
| Client host | Windows desktop executable | Not started | Add Android lifecycle/Activity host |
| Input | Keyboard/mouse | Not implemented | Add touch-to-action mapping without changing server authority |
| Rendering | DesktopGL/SDL/OpenGL desktop assets | Not validated | Prove Android graphics backend with existing content |
| Audio | OpenAL desktop resources | Not validated | Verify Android audio backend and lifecycle |
| Networking | LiteNetLib UDP path | Protocol likely reusable; Android unvalidated | Connect to unmodified baseline server |
| Authentication | Existing client/server flow | Unvalidated | Test unchanged protocol after mobile host exists |
| File/settings | Desktop current-directory/resource paths | Blocked | Map to Android app-private/scoped storage |
| Editor | Windows Forms/Windows target | Not applicable | Keep desktop-only and isolated |
| Server | Desktop/server package | Not applicable | Keep unchanged for compatibility |
| Tests | Projects restore/build; execution blocked by missing .NET 8 runtime | Not started | Install approved .NET 8 runtime/workload, then run tests |
