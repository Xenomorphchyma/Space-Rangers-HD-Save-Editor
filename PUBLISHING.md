# GitHub publishing checklist

## Repository settings

- Owner: `Xenomorphchyma`
- Repository: `Space-Rangers-HD-Save-Editor`
- Default branch: `main`
- Description: `Open-source save editor for Space Rangers HD: A War Apart on Windows.`
- Topics: `space-rangers-hd`, `save-editor`, `winforms`, `dotnet-framework`,
  `modding`, `game-tools`
- Issues: enabled
- Actions: enabled
- Private vulnerability reporting: enabled

Create the GitHub repository empty: do not generate another README, license or
`.gitignore`, because all of them are already present locally.

## Before the first push

Review `NOTICE.md` and `THIRD-PARTY-NOTICES.md`, especially the licensing scope
of UI compatibility data. Configure a real Git author email locally; do not put
an email into repository files unless it is intended to be public.

```powershell
git config user.name "Xenomorphchyma"
git config user.email "YOUR_VERIFIED_OR_NOREPLY_GITHUB_EMAIL"
git add --all
git status
git commit -m "Initial open-source release"
git remote add origin https://github.com/Xenomorphchyma/Space-Rangers-HD-Save-Editor.git
git push -u origin main
```

## First release

The `publish-release` workflow verifies and packages tags matching `v*`, then
creates a GitHub Release containing the ZIP and `manifest.json`.

```powershell
git tag -a v1.0.0-rc.1 -m "Space Rangers HD Save Editor 1.0.0-rc.1"
git push origin v1.0.0-rc.1
```

Before tagging, verify that the `build-and-test` workflow is green. Recommended
branch protection for `main`: require pull requests and the `windows` job from
`build-and-test`, dismiss stale approvals, and block force pushes/deletions.
