# server sider

A \[ server-side / host-only \] mod that adds small gameplay changes.

## compatibility

This mod should be compatible with the *Seekers of the Storm* update ***and*** earlier patches.

However, I don't have the time to test on downpatched game versions *— please report any issues to the [GitHub repository](https://github.com/itsschwer/ror2-serversider/issues)!*

## gameplay changes

> The configuration file is automatically reloaded at the start of each run.
>
> *[***OptionGenerator***](https://thunderstore.io/package/6thmoon/OptionGenerator/) *(+[***Risk Of Options***](https://thunderstore.io/package/Rune580/Risk_Of_Options/))* can be used to change the configuration in-game, rather than editing the file or using **r2modman**.*

Name | Effect | Enabled by default?
---    | ---    | ---
`rescueShipPortal` | Spawns a portal inside the *Rescue Ship* to allow looping after defeating *Mithrix*.  | `true`
`pressurePlateGracePeriod` | The length of time (in seconds) that a pressure plate *(Abandoned Aqueduct)* will remain pressed after being activated.<br/><br/>Zero disables this functionality (reverts to vanilla behaviour).<br/>Negative values make pressure plates stay down forever once pressed.<br/><br/>*No more pot rolling or waiting for friends!* | `30` *(true)*
`quitToLobbyButton` | Adds a "Quit to Lobby" button to the Run pause menu. <br/><br/> *Useful in multiplayer for changing characters/survivors, difficulty, or artifacts without disconnecting everyone from the lobby.* | `true`
`voidPickupConfirmAll` | Always require confirmation *(i.e. Interact input)* to pick up void items. <br/><br/> *Intended to encourage sharing void items in multiplayer.* | `true`
`voidFieldFogAltStart` | Changes the *Void Fields*' fog to only become active once a *Cell Vent* has been activated *(rather than on entry)*. | `false`
`chanceDollMessage` | Rewords the *Shrine of Chance* success message to indicate if a *Chance Doll* affected the reward.<br/><br/>*This tweak is redundant after **RoR2v1.3.6 [Seekers of the Storm Roadmap Phase 1 — Items & Elites]**, as this is now implemented in vanilla.* | `false`
`teleportOutOfBoundsPickups` | Teleports items that fall off the map to the nearest valid ground node. | `true`
`preventEarlyHalcyonShrineActivation` | Disables the "Pray to Halcyon Shrine" prompt to prevent activating the shrine before it is fully charged. <br/><br/> *Useful for preventing accidentally ending the Halcyon Shrine early when trying to interact with other nearby shrines, pickups, etc.* | `true`

### chat additions

Name | Effect | Enabled by default?
---    | ---    | ---
`sendItemCostInChat` | Sends a chat notification listing the items that are consumed when a *Scrapper*, *3D Printer*, *Cleansing Pool*, or *Cauldron* is used. | `true`
`includeScrapInItemCost` | Includes *Item Scrap* in the list printed by `sendItemCostInChat`. | `false`

## screenshots

### `rescueShipPortal`
![rescue ship portal sample screenshot](https://github.com/itsschwer/ror2-serversider/blob/main/xtra/demo-rescue-ship-portal.png?raw=true)

### `pressurePlateGracePeriod`
> *config: 3s*\
![gif demonstration of pressure plate grace period](https://github.com/itsschwer/pressure-drop/blob/main/xtra/demo-pressure-plate-timed.gif?raw=true)

### `quitToLobbyButton`
![quit to lobby button sample screenshot](https://github.com/itsschwer/ror2-serversider/blob/main/xtra/demo-quit-to-lobby-button.png?raw=true)

### `chanceDollMessage`
![chance doll message sample screenshot](https://github.com/itsschwer/ror2-serversider/blob/main/xtra/demo-chance-doll-message.png?raw=true)

### `sendItemCostInChat`
![printing, scrapping, reforging, and cleansing chat message screenshot](https://github.com/itsschwer/ror2-serversider/blob/main/xtra/demo-item-cost-in-chat.png?raw=true)

## see also

- [QuickRestart](https://thunderstore.io/package/AceOfShades/QuickRestart/) <sup>[*src*](https://github.com/Maceris/quick-restart)</sup> by [AceOfShades](https://thunderstore.io/package/AceOfShades/) — alternative implementation of `quitToLobbyButton`
    - uses custom UX
    - also has a "restart" button to start a new run without returning to the lobby
- [OutOfBoundsItemsFix](https://thunderstore.io/package/rob_gaming/OutOfBoundsItemsFix/) by [rob_gaming](https://thunderstore.io/package/rob_gaming/) — alternative implementation of `teleportOutOfBoundsPickups`
    - based on fall distance rather than out of bounds map zones
