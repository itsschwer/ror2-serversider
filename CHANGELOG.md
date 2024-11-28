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
