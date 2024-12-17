## 1.6.1
- Change the default value for `chanceDollMessage` to `false`
    - ***Risk of Rain V1.3.6 [Seekers of the Storm Roadmap Phase 1 — Items & Elites]** introduces a vanilla implementation of this tweak*
        - `<color=#53ff00>{0} offered to the shrine and was greatly rewarded!</color>`, rather than `<style=cShrine>{0} offered to the shrine and was rewarded greatly!</color>`
            - *Supports second-person POV (i.e. uses "You" instead of player name)*
            - *Message is coloured green*
        - Can still be enabled, but the message will lose the second-person POV and be a bit nonsensical
            - ...`and was greatly rewarded greatly!`
        - Tweak will be retained in case of downpatching

## 1.6.0
- Port features from [PressureDrop](https://thunderstore.io/package/itsschwer/PressureDrop/) <sup>[***src***](https://github.com/itsschwer/pressure-drop)</sup>
    - `voidPickupConfirmAll`
- Refactor `rescueShipPortal` positioning logic to be relative to the mesh game object
    - *Should now work better with mods that place their own `Moon2DropshipZone` prefab instances*

## 1.5.0
- Add `preventEarlyHalcyonShrineActivation`
- Adjust `teleportOutOfBoundsPickups` logic

### 1.4.1
- Fix pre-emptive hooking of `pressurePlateGracePeriod` methods

## 1.4.0
- Port features from [PressureDrop](https://thunderstore.io/package/itsschwer/PressureDrop/) <sup>[***src***](https://github.com/itsschwer/pressure-drop)</sup>
    - `sendItemCostInChat`
        - `includeScrapInItemCost`
    - `pressurePlateGracePeriod` *(formerly `pressurePlateTimer`)*

## 1.3.0
- Add `teleportOutOfBoundsPickups`

## 1.2.0
- Add `exitToLobbyButton`
- Fix `chanceDollMessage` erroneously using the `rescueShipPortal` configuration option
- Reword the `rescueShipPortal` prompt to differentiate from the *Green Portal (Seekers of the Storm)* prompt
- Rewrite how hooks and configuration options are managed

## 1.1.0
- Add `chanceDollMessage`
- Update icon
- Start assembly versioning

# 1.0.0
- Initial release
