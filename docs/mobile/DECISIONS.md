# Mobile decisions

## D-001: Separate runtime workspaces

Use `C:\Studio\Intersect\Engine-Development` for source/build/research and `C:\Studio\Intersect\Game-Development` for the stable v0.8.0-beta.581 package. Do not copy experimental binaries or content between them.

**Reason:** the editor/server/client must remain usable while mobile work changes source and build outputs.

## D-002: Preserve server protocol

Do not modify authoritative server behavior for the first Android proof of concept. Reuse the existing networking and authentication protocol.

**Reason:** mobile compatibility should be demonstrated against an unmodified server.

## D-003: Prefer an adapter over a rewrite

Keep client-core/game logic reusable and isolate Android lifecycle, storage, input, rendering backend, and audio integration behind platform-specific adapters.

**Reason:** the current client has reusable network/game code but directly embeds desktop assumptions.

## D-004: No Android implementation before toolchain proof

First establish a minimal Android build using a compatible backend and record the exact toolchain. Do not introduce a major rewrite based only on package names or assumptions.
