# Repository Guidelines

## Project Structure & Module Organization
- `src/Pims.App` hosts the WPF shell, tray integration, and menu orchestration (email, OTP, FTP, credentials).
- `src/Pims.Core` supplies authentication, HTTP session, email, and FTP services; prefer adding reusable logic here so it stays testable.
- `tests/Pims.Tests` mirrors `Pims.Core` namespaces, covering cookie rotation, OTP parsing, and other headless behaviors.
- Runtime data (`appsettings.pims.json`, `credentials.json`, `preferences.json`) lives under `%APPDATA%/Pims`; seed files are auto-created with sensible defaults.

## Build, Test, and Development Commands
- `dotnet restore` hydrates NuGet dependencies whenever solution references change.
- `dotnet build src/Pims.App/Pims.App.csproj -c Debug` compiles the WPF front end; swap to `-c Release` when sharing binaries with coworkers.
- `dotnet test tests/Pims.Tests/Pims.Tests.csproj -c Debug` validates OTP parsing and other core behaviors; add tests beside new services.
- `dotnet run --project src/Pims.App` launches the utility hub (run on Windows so tray icons, hotkeys, and notifications work).

## Authentication & Session Flow
- Startup runs the primary HTTP POST login, storing `Set-Cookie` values in a shared `CookieContainer`; all subsequent HTTP requests reuse it.
- On failure, the app retries with the secondary password automatically; both passwords plus the user ID persist via `credentials.json` so users never type them again.
- Credentials can be edited from the in-app menu; saving them resets the cached session and forces the next login to reuse the new values.

## Tray, Menus & Hotkeys
- Tray context menu mirrors every in-app menu entry and keeps the app resident in the background; double-click restores the main window.
- Menu activations always create fresh windows—switching between utilities never rehydrates prior state, and opening the same menu twice gives isolated sessions.
- The global shortcut (default `Ctrl+Alt+O`, configurable in preferences) runs the OTP capture workflow even when the main window is hidden.

## Email & OTP Workflow
- Email queries use the authenticated session; `SessionHttpClient` auto-refreshes cookies when the server signals expiry and retries the request.
- The OTP helper grabs the latest subject line, extracts a numeric code, copies it to the clipboard, then shows a Windows toast including the OTP.

## File Transfer Utility
- The FTP publisher remembers the last local JAR path and remote directory in `preferences.json`; subsequent uploads pre-fill those values.
- Uploads reuse the stored user credentials, copying the selected JAR to the configured FTP directory without extra protocol hardening.

## Commit & Collaboration Notes
- Follow Conventional Commits, tagging areas such as `feat(auth)`, `fix(email)`, or `chore(tray)` for quick history scans.
- Document manual checks (`dotnet test`, OTP shortcut, tray launch, FTP upload) in PRs or shared diffs before shipping binaries to colleagues.
