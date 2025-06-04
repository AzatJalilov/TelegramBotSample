# Codex Agent Guidelines

This repository uses .NET 8 projects with a small test suite.

## Commit messages
- Provide a short summary line describing the change.
- Use the present tense ("Add feature" not "Added feature").

## Testing
- Run `dotnet test` from the repository root before committing any changes.
- If tests fail because packages cannot be restored in this environment, note this in the PR description.
- Keep overall test coverage at **90%** or higher.

## Style
- Keep code changes minimal and clear.
- Prefer built-in .NET formatting conventions.
