# Hunt Automator

Experimental Dalamud API 15 hunt-orchestration plugin.

> **Account-risk notice:** HuntAutomator automates gameplay. Square Enix prohibits botting/automation, so using this plugin can expose an FFXIV account to enforcement action. Use at your own risk.

## Install from the custom repository

After this project has been pushed to a public GitHub repository and its first release workflow has completed, add the generated custom repository URL in Dalamud:

```text
https://raw.githubusercontent.com/Antipokemon/HuntAutomator/main/repo.json
```

Then open `/xlplugins`, search for **Hunt Automator**, and install it normally.

See [CUSTOM_REPOSITORY.md](CUSTOM_REPOSITORY.md) for the one-time GitHub setup and release process.

## What it does

- Reads currently accepted daily clan/mark bills directly from the game's `MobHunt` state and Lumina `MobHuntOrder` data.
- Reads weekly elite/B-rank bills the same way.
- Groups targets into a queue and automatically changes zones with Lifestream IPC.
- Searches every bundled spawn cluster for a daily mark as a fast path, then falls back to a deterministic full-map patrol.
- B-ranks use a full-map patrol because an elite mark can occupy many spawn points.
- Continuously scans `IObjectTable` by `NameId`; once visible, selects the target and navigates into the configured combat range.
- Mounts for distant visible copies of the same daily target instead of approaching the next kill entirely on foot.
- Uses vnavmesh to snap patrol points to the navmesh and path to them.
- Approaches and dismounts near the mob, initiates auto-attack, and starts RotationSolverReborn in Auto (Big) mode; stops RSR after the kill or timeout.
- Verifies daily completion from the game's hunt-bill kill counter rather than assuming a despawn means credit.
- Optionally imports the currently recorded HuntHelper train through `HH.GetTrainList` and queues its live entries as A-rank targets.
- Stops on player death rather than automatically attempting resurrection.

## Dependencies

Required:

- Dalamud API 15
- vnavmesh
- RotationSolverReborn
- Lifestream (`Lifestream.Teleport` IPC)

Optional:

- HuntHelper for A-rank train import
- HuntBuddy for its normal bill/map UI; Hunt Automator does not need it to read bills
- BossMod Reborn may remain installed for encounter overlays/mechanics; this build uses RotationSolverReborn for combat execution.

## Commands

- `/hauto` — open UI
- `/hauto start` — read bills/train and start
- `/hauto stop` — stop navigation/combat and clear the queue
- `/hauto reload` — rebuild the active queue

## Build locally

With Dalamud installed and the Dalamud SDK available:

```bash
dotnet build HuntAutomator.csproj -c Release
```

CI follows the current official SamplePlugin pattern and downloads the latest Dalamud development distribution before building.

## Development status

The source was designed against APIs inspected in August 2026. Third-party IPC and commands can change. The project should be integration-tested in FFXIV before unattended use, especially after updates to Dalamud, vnavmesh, RotationSolverReborn, Lifestream, or HuntHelper.
