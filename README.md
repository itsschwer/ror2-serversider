# server sider

A \[ server-side / host-only \] mod that adds small gameplay changes.

## compatibility
This mod should be compatible with the *Seekers of the Storm* update ***and*** earlier patches.

However, I don't have the energy to test on downpatched game versions *— please report any issues to the [GitHub repository](https://github.com/itsschwer/ror2-serversider/issues)!*

## gameplay changes

> The configuration file is automatically reloaded at the start of each run.
>
> *[***OptionGenerator***](https://thunderstore.io/package/6thmoon/OptionGenerator/) *(+[***Risk Of Options***](https://thunderstore.io/package/Rune580/Risk_Of_Options/))* can be used to change the configuration in-game, rather than editing the file or using **r2modman**.*

Name | Effect | Enabled by default?
---    | ---    | ---
`rescueShipPortal` | Spawns a portal inside the *Rescue Ship* to allow looping after defeating *Mithrix*  | `true`
`voidFieldFogAltStart` | Changes the *Void Fields*' fog to only become active once a *Cell Vent* has been activated *(rather than on entry)* | `false`
`chanceDollMessage` | Rewords the *Shrine of Chance* success message to indicate if a *Chance Doll* affected the reward | `true`
`quitToLobbyButton` | Adds a "Quit to Lobby" button to the Run pause menu. <br/><br/> Useful in multiplayer for changing characters/survivors, difficulty, or artifacts without disconnecting everyone from the lobby. | `true`

## screenshots

### `rescueShipPortal`
![rescue ship portal sample screenshot](https://github.com/itsschwer/ror2-serversider/blob/main/xtra/demo-rescue-ship-portal.png?raw=true)

### `chanceDollMessage`
![chance doll message sample screenshot](https://github.com/itsschwer/ror2-serversider/blob/main/xtra/demo-chance-doll-message.png?raw=true)

### `quitToLobbyButton`
![quit to lobby button sample screenshot](https://github.com/itsschwer/ror2-serversider/blob/main/xtra/demo-quit-to-lobby-button.png?raw=true)

## see also

- [PressureDrop](https://thunderstore.io/package/itsschwer/PressureDrop/) — my initial server-side mod, geared towards "friendlier multiplayer" *(but perfectly usable for singleplayer too)*

<!--
- [Artifactor](https://thunderstore.io/package/itsschwer/Artifactor/) <sup>[*src*](https://github.com/itsschwer/ror2-artifactor)</sup> — turns `rescueShipPortal` into an *Artifact*
-->
