# IRacing-TV-2 Inspired Overlay Approach

This repository already includes a lightweight overlay host (`OverlayHost`) with widgets (`StandingsTowerWidget`, `DriverInfoWidget`) and a browser mockup. To align with the feature set of [IRacing-TV-2](https://github.com/mherbold/IRacing-TV-2) while keeping this codebase focused on iRSDK telemetry, follow this integration outline.

## Goals
- Reuse IRSDKSharper telemetry reading and widget rendering logic.
- Mirror IRacing-TV-2's production workflow: dedicated telemetry daemon, broadcast overlay browser sources, and a control/producer UI.
- Keep the system modular so the producer/overlay clients can run remotely or inside OBS browser sources.

## Architecture Sketch
1. **Telemetry Daemon (C#)**
   - Build a headless console app (similar to `OverlayConsoleApp`) that subscribes to iRacing via `IRacingSdk` and publishes a normalized state over WebSockets.
   - Use `OverlayTelemetryReader` to map raw arrays to strongly typed models, then serialize `OverlayState` snapshots at a fixed interval (e.g., 10 Hz) for overlays and producer UI.
   - Keep the daemon separate from the browser clients (akin to IRacing-TV-2's data service).

2. **Overlay Client (Browser/OBS Source)**
   - Reuse `OverlayWeb/overlay.html` and refactor `overlay.js` to consume WebSocket snapshots instead of sample data or `BroadcastChannel` messages.
   - Render `StandingsTowerWidget` and `DriverInfoWidget` visuals driven by incoming JSON; use CSS tokens in `OverlayWeb/styles.css` for theming.
   - Allow per-source configuration via query params (e.g., `?showTower=true&focusCar=123`).

3. **Producer Panel (Browser App)**
   - Extend `OverlayWeb/producer.html` to connect to the same WebSocket feed.
   - Send control messages (focus car, tower visibility, theme toggles) over a separate WebSocket channel or by POSTing to the daemon.
   - Broadcast producer actions to overlays using `BroadcastChannel` (when co-located) or WebSocket fan-out.

4. **Message Contracts**
   - Define a shared schema for telemetry snapshots and producer commands under `Overlay/OverlayModels.cs` to keep C# and JS aligned.
   - Example snapshot shape:
     ```json
     {
       "session": {"name":"Race","lap":25},
       "leaderboard": [{"carIdx":1,"pos":1,"driver":"Jane Doe","gap":"+0.0"}, ...],
       "focus": {"carIdx":5,"driver":"John Smith","team":"TeamX","lic":"A4.50","ir":3120,
                  "lastLap":"1:32.450","delta":"-0.120","sr":4.50}
     }
     ```
   - Example command payload:
     ```json
     {"type":"setFocus","carIdx":42}
     {"type":"toggleTower","visible":true}
     ```

5. **Deployment Workflow**
   - Run the telemetry daemon on the iRacing host to reduce latency.
   - Point OBS browser sources at `overlay.html` served by a static file server (or `file://`) and configured with the daemon WebSocket URL.
   - Open the producer panel in a browser; it will subscribe to telemetry and publish control commands.

## Next Steps
- [ ] Add a WebSocket server to the console app that streams `OverlayState` snapshots.
- [ ] Refactor `OverlayWeb/*.js` to consume live WebSocket data and commands.
- [ ] Add a JSON schema contract shared between C# and JS for telemetry and commands.
- [ ] Provide Docker/Windows scripts to start the daemon plus a static file server for the web overlays.

This approach keeps the current C# telemetry code while adopting the distributed overlay/producer flow demonstrated by IRacing-TV-2.
