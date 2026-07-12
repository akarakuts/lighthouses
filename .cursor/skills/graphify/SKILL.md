---
name: graphify
description: Keeps the graphify knowledge graph in sync after code changes. Use when editing source files (.cs, .sh, .yml, .md in Assets/, Tools/, .github/), after completing an implementation task, or when the user mentions graphify, architecture graph, or graphify-out.
---

# Graphify auto-update

This project uses [graphify](https://github.com/Graphify-Labs/graphify) for a code knowledge graph in `graphify-out/`.

## Mandatory after code edits

When you **create, modify, or delete** project source files, run **before ending the turn**:

```bash
export PATH="$HOME/.local/bin:$PATH"
graphify update .
```

- AST-only rebuild — no API key, usually a few seconds.
- Skip only when the session touched **no** code/config (docs-only typo in README with no structural change is optional).
- If `graphify-out/graph.json` is missing, run `graphify update .` once before exploring the codebase.

## When exploring code

Prefer graph tools over blind grep when the graph exists:

```bash
graphify query "<architecture question>"
graphify path "<SymbolA>" "<SymbolB>"
graphify explain "<concept>"
```

Read `graphify-out/GRAPH_REPORT.md` for a high-level map. Open `graphify-out/graph.html` for the interactive view.

## Git hooks (local, not in repo)

Post-commit rebuild is installed via `graphify hook install` in `.git/hooks/`. After clone, run once:

```bash
graphify hook install
```

## Troubleshooting

| Problem | Fix |
|---------|-----|
| `graphify: command not found` | `uv tool install graphifyy` then ensure `~/.local/bin` is on PATH |
| Stale graph | `graphify update .` |
| Hook not running | `graphify hook install` from project root |
