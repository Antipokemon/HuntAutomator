# Custom Repository Publishing

This repository is set up to publish HuntAutomator as a Dalamud custom repository.

## First-time GitHub setup

1. Create a **public** GitHub repository, preferably named `HuntAutomator`.
2. Push this directory to the repository's default branch (`main` is recommended).
3. Open **Settings → Actions → General** and make sure workflows are allowed to run.
4. Under **Workflow permissions**, allow **Read and write permissions** if your organization/account policy does not already permit the workflow's requested `contents: write` permission.
5. Publish the first release by creating and pushing a version tag:

   ```bash
   git tag v0.2.0
   git push origin v0.2.0
   ```

   You can alternatively run **Actions → Release and Publish Custom Repository → Run workflow** and enter `0.2.0`.

The release workflow will:

- set the plugin's assembly version from the release version;
- download the current Dalamud development distribution;
- build the plugin on a Windows GitHub runner;
- package `HuntAutomator.zip`;
- create/update the matching GitHub Release;
- generate a Dalamud repository manifest; and
- publish that manifest to a dedicated `plugin-repo` branch.

## Repository URL to add in Dalamud

After the first successful release, add this URL under Dalamud's **Custom Plugin Repositories**:

```text
https://raw.githubusercontent.com/<YOUR_GITHUB_USER>/HuntAutomator/plugin-repo/repo.json
```

If you use a different GitHub repository name, replace `HuntAutomator` accordingly.

The URL is deliberately on the `plugin-repo` branch. Do not manually maintain that branch; the release workflow replaces it on each release.

## Releasing updates

Make your source changes, commit/push them, then publish a higher version:

```bash
git tag v0.2.1
git push origin v0.2.1
```

Dalamud compares `AssemblyVersion`, so each update must use a version higher than the previous release.

## Why the distribution endpoint must be public

Dalamud custom repository URLs are fetched with a normal HTTP GET and do not support authentication/authorization. A public GitHub repository provides anonymous access to both the raw `repo.json` and the GitHub Release ZIP.
