Your task is to “onboard” this repository to the Copilot coding agent by adding this .github/copilot-instructions.md file. It gives the agent the minimum, durable context it needs to understand the project, build it, validate changes, and open mergeable PRs.

These instructions are repository-wide and task-agnostic. If something here contradicts ad-hoc guidance in an issue or PR, prefer the ad-hoc guidance for that task.

<Goals>
- Keep generated PRs mergeable by building and validating locally before proposing changes.
- Minimize broken shell/PowerShell steps by pinning prerequisites and the order of operations.
- Reduce unnecessary codebase exploration by pointing to the right files and patterns first.
</Goals>

<Limitations>
- Do not include issue-specific plans or debugging transcripts.
- Keep guidance concise (≈2 pages). Link to existing scripts/configs in-repo rather than inlining them.
- Localization policy: Base language is Chinese (Simplified). English is also maintained by the core team; all other languages are community-contributed via Crowdin. If a PR introduces new UI strings, only add the new strings to the Chinese resource file at src/Snap.Hutao/Snap.Hutao/Resource/Localization/SH.resx. Do not edit any other locale files (including English) in that PR; leave them untranslated.
</Limitations>

---

## High-level overview

- What this is. Snap Hutao (胡桃工具箱) is a C# WinUI 3 desktop application packaged as MSIX. It’s an open-source toolkit that enhances the Genshin Impact experience on Windows without modifying the game client.
- Target OS. Runs on Windows 11 (22H2+) and Windows 10 22H2 with the January 2025 update (Build 19045.5371, KB5049981). Agent work should assume a modern, fully updated Windows dev box.
- Key tech. .NET 9 (C#), Windows App SDK / WinUI 3 (XAML), GitHub Actions CI. The repo uses analyzers (e.g., StyleCop), and common validation utilities (e.g., Microsoft.VisualStudio.Validation).
- License & scope. MIT. The app integrates official game data and community features; it must not introduce destructive client modifications.

Rule of thumb: Prefer Visual Studio 2022 for build/package/sign workflows. Use the .NET SDK CLI for restore/build/test where practical.

---

## Repository layout (what to look for first)

- Top level
  - .github/ — issue templates/workflows; this file lives here.
  - res/ — assets and misc resources.
  - src/Snap.Hutao/ — the solution root for the desktop app (primary code lives here).
  - Build/CI config in the root (e.g., build.cake, NuGet.Config, CI YAML).

- Inside src/Snap.Hutao
  - Solution: Snap.Hutao.sln (primary).
  - Main app project: Snap.Hutao/ (WinUI 3 XAML + C#).
  - Common folders you’ll work in:
    - UI/ (XAML views, controls) and ViewModel/ (MVVM view models).
    - Service/ (app/business services; network, storage, background work).
    - Model/ (domain/data models).
    - Core/ and Extension/ (infrastructure, helpers, extensions).
    - Web/ (HTTP/API integrations) and Win32/ (interop).
  - Important config:
    - App.xaml / App.xaml.cs (app startup/resources), Package.appxmanifest (MSIX).
    - .editorconfig, stylecop.json, GlobalUsing.cs, BannedSymbols.txt (coding standards & guardrails).

Search shortcuts: “Package.appxmanifest” for capabilities/identity, “stylecop.json” for style rules, “GlobalUsing.cs” for shared namespaces, and “*.xaml” + “ViewModel” for MVVM entry points.

---

## Branching & PR policy (follow strictly)

- Work on develop for feature and bug-fix branches; target PRs to develop.
- CI builds main, develop, and feat/* branches. Alpha artifacts may be produced by CI for testing.
- Link issues with closing keywords in PR descriptions when appropriate (e.g., Fixes #123).

---

## Prerequisites (Windows only)

1) Windows: Windows 11 22H2+ or Windows 10 22H2 Build 19045.5371 (KB5049981). Enable Developer Mode.
2) Visual Studio 2022 with workloads:
   - .NET desktop development
   - Desktop development with C++
   - Windows application development
3) MSIX tooling: Single-project MSIX Packaging Tools for VS 2022 (install if your VS version doesn’t include it).
4) Optional (runtime features & UX): WebView2 Runtime, Segoe Fluent Icons font, MSVC Runtime (for some features).

If the agent runs headless, prefer CLI restore/build/test for validation and rely on CI to produce MSIX artifacts.

---

## Build, run, and validate (minimal, reliable sequence)

Always do steps in this order on a clean clone:

1) Restore
    dotnet restore src/Snap.Hutao/Snap.Hutao.sln

2) Build (Debug)
    dotnet build src/Snap.Hutao/Snap.Hutao.sln -c Debug

   Expect WinUI 3 XAML compilation and analyzer checks. Fix all analyzer violations before proposing changes.

3) Run (developer install via VS)
   - Open src/Snap.Hutao/Snap.Hutao.sln in Visual Studio 2022.
   - Set the main project as startup, select x64, press F5 (packaged debugging will register the app locally).

4) Package (when needed)
   - Use Visual Studio’s Publish / Package for MSIX.
   - CI may also produce Alpha packages for main/develop/feat/*. Local install of CI-signed packages can require installing the provided certificate.

5) Tests (if present)
    dotnet test src/Snap.Hutao/Snap.Hutao.sln

   Add or update tests for non-UI logic when you change behavior.

Common validation (run before opening a PR):
- Solution builds cleanly (no warnings from analyzers if the repo treats warnings as errors for StyleCop).
- App starts in Debug; smoke-test the feature you touched.
- No changes to Package.appxmanifest capabilities unless required by the task and documented in the PR.

---

## Coding style & contribution patterns

- MVVM: Put UI behavior in ViewModels; keep XAML code-behind minimal. Use async APIs for I/O and keep the UI thread responsive.
- Consistency first: Mirror existing folder structure, naming, and patterns. Prefer existing services/helpers over new global/singleton patterns.
- Analyzers: Fix StyleCop and other analyzer diagnostics. Follow rules set in .editorconfig / stylecop.json (naming, docs, layout).
- Validation & errors: Use established guard/validation utilities; fail fast on bad inputs; handle network/storage exceptions gracefully.
- XAML: Reuse existing styles/resources; keep bindings simple and observable; avoid UI thread blocking.
- Security & privacy: Don’t log secrets or game tokens; follow existing storage conventions.

---

## Where to implement changes (quick map)

- New UI feature → UI/ (XAML) + ViewModel/ (presentation) + Service/ (logic/data) + Model/ (types).
- Network/API work → Web/ (HTTP clients, DTOs) + relevant Service/.
- Interop → Win32/ (P/Invoke/wrappers). Be conservative and follow precedent to avoid regressions.
- Cross-cutting → Core/ & Extension/ for helpers, DI, and reusable infrastructure.

---

## CI interaction (what the agent should expect)

- Pushes to feature branches trigger build checks; some branches produce Alpha artifacts.
- If CI fails on analyzers or formatting, align code with the repo’s rules rather than suppressing diagnostics.
- Treat CI as the source of truth for packaging/signing; keep local steps minimal and deterministic.

---

## Agent operating guidelines

- Trust this file first. Only explore or run extra commands if information is missing or build errors indicate a mismatch.
- Be conservative with tooling changes. Don’t alter SDK/target framework, packaging, or signing settings unless the task explicitly requires it.
- Keep edits scoped. Touch the smallest set of files; update tests/docs alongside code when you change behavior.
- PR hygiene. Use clear commit messages, link issues with keywords, and summarize validation steps you performed (build, tests, manual smoke).

---