# Deeplight (F26)

A Unity project built by the Generate Product Development Studio game dev team. Made with love for Deeplight Games.

## Unity Version

**Unity 6 (6000.4.6f1)**

Make sure your local Editor install matches this exact version. Opening the project in a different version can trigger forced reserialization of scenes/prefabs and create noisy, unrelated diffs.

## Branch Structure

```
main   — source of truth, production-ready
test   — approved PRs land here for QA before merging into main
<ticket-branches>  — one branch per dev ticket, branched off test (naming convetion: "s[sprint #]-[short descirption")
```

**Workflow:**

1. Create a ticket branch off `test` (e.g. `S1-EnemyBaseClass`).
2. Open a PR back into `test` when ready for review.
3. Kevin Mat reviews (code + scene/prefab check in-editor) and merges into `test`.
4. Once verified stable, `test` is merged into `main`.

**Moving files (scripts, prefabs, scenes):** always move via drag-and-drop inside the Unity Editor's Project window, never through the OS file explorer or a raw terminal `mv`. Unity links scenes/prefabs to scripts by GUID (stored in the `.meta` file), not by file path — moving in-editor keeps the `.meta` paired correctly. If you must move via terminal, move the `.cs`/`.prefab` file and its matching `.meta` together, then verify in Unity that nothing shows as "Missing (Mono Script)" before committing.

## Project Structure

```
Assets/
  Scripts/
    Player/       — player-only scripts (controller, input handling)
    Enemies/      — enemy base classes and concrete enemy types
    Combat/       — projectiles, weapons, shooting logic
    Health/       — health/damage components
    Utilities/    — generic, reusable helpers (not tied to one feature)
    Input/        — shared input assets/config, if used by more than the player
  Prefabs/
    Characters/
    Enemies/
    Combat/
    ...           — organized by feature, not by which script it uses
  Scenes/
    Ife/          - Scenes for Ife to work on and test tickets.
    Chuck/        - Scenes for Chuck to work on and test tickets.
    Dreshta/      - Scenes for Dreshta to work on and test tickets.
    Vincent/      - Scenes for Vincent to work on and test tickets.
```

Scripts are grouped by **gameplay feature**, not by file type. Prefabs are grouped by **what they are in the game**, and don't need to mirror the Scripts folder structure 1:1.

## Coding Standards

- **Default to `private`.** Only make something more visible when another class genuinely needs it.
  - `[SerializeField] private` — Inspector-editable, but not accessible from other scripts.
  - `protected` — for base classes (e.g. `EnemyBase`) that subclasses need to read/write or override.
  - `public` — only when another class or the Inspector truly needs external access.
- **Remove empty Unity lifecycle methods** (`Start()`, `Update()`, etc.) if they have no body — each one costs Unity a small per-frame dispatch cost even when empty. Exception: keep an empty method as `protected virtual` in a base class if it's an intentional extension point for subclasses to override.
- **Give tunable fields sensible non-zero defaults.** A blank/zero default (e.g. a check radius) can fail silently with no error — always default to a value that fails safe.
- **Use XML Documentation for all classes and methods.**

## PR Review Checklist

- [ ] Code reviewed for logic, style, and encapsulation (`private`/`protected`/`public` used deliberately)
- [ ] No unintended `.meta`/GUID changes
- [ ] No new files landed loose outside the established folder structure
- [ ] Branch checked out and opened in Unity locally (not just reviewed as a diff)
- [ ] Affected scene(s) opened — hierarchy sane, no Console errors on load
- [ ] Feature played end-to-end in Play mode
- [ ] No "Missing (Mono Script)" on anything the PR touched

