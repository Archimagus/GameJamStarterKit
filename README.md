## GameJamStarterKit
Fast-start Unity starter kit for game jams: prebuilt main menu, pause menu, audio system (with auto-enum of clips), settings panels, a lightweight event system, and scriptable variables. Drop in your scenes and assets, hook up a few references, and ship.

### What you get
- **Menus**: Main menu and pause menu prefabs, stack-based menu navigation, input-ready UI focus.
- **Audio**: Central `AudioManager`, Unity `AudioMixer` preset, UI SFX helper, automatic `AudioClips` enum generation and database.
- **Settings**: Panels for audio (volume sliders) and graphics (quality, fullscreen mode, resolution choices).
- **Events**: Scriptable `GameEvent` + `GameEventListner` with optional payloads (`EventData`).
- **Variables**: Scriptable variables with editor UX (`*_Reference` types, constant/variable toggle, optional clamping, reset-on-scene).
- **Editor tools**: Scenes picker window, one-click multi-platform build, property drawers for scenes, events, and variables.

---

## Quick start
1) Open the project in Unity (URP is already set up). When prompted, let Unity update the project if needed.
  - TextMesh Pro essentials:
    - Open the main menu scene.
    - In the Hierarchy, select `HeaderText` to trigger TMP import; allow importing Essentials.
    - You may see console errors initially; they usually clear after import.
    - If button labels don’t appear, restart Unity.

2) Scenes
- Use `Assets/Scenes/MainMenu.unity` and `Assets/Scenes/GameScene.unity` as starting points.
- Ensure there is exactly one `Game Manager` in your gameplay scene. If missing, drag `Assets/Prefabs/Game Manager.prefab` into the scene root.

3) Scriptable objects (already included, just verify)
- `Assets/ScriptableObjects/Utilities/GameData.asset`: assign your main menu and game scenes to its fields.
- `Assets/ScriptableObjects/Utilities/GameTime.asset`
- `Assets/ScriptableObjects/Utilities/MenuStack.asset`
- `Assets/ScriptableObjects/Utilities/AudioDatabase.asset` (auto-filled by the audio postprocessor)

4) Input
- There is a button mapping named `Menu` in Project Settings > Input Manager (bound to Escape). It toggles the pause menu in play.
- Default axes like `Vertical` are used for UI focus when using keyboard/controller.

5) Play
- Press Play from `MainMenu`. Keyboard/controller navigation is pre-configured; Escape will close settings or quit from the main menu.

---

## Menus and navigation
- **Main menu**: See `Assets/Prefabs/UI/Menu/Main Menu Canvas.prefab`. The menu selects `NewGameButton` when navigated via keys/gamepad.
- **Pause menu**: See `Assets/Prefabs/UI/Game/PauseMenu.prefab`. `GameManager` toggles pause on the `Menu` input and shows `PauseMenu.Instance.PausePanel`.
- **Menu stack**: The `MenuStack` ScriptableObject manages a stack of menus.
  - `OpenMenu(menu, diableCurrent: bool)` pushes a menu and optionally disables the current menu.
  - `CloseMenu(out int closed)` pops one or more menus and restores the previous; when the stack becomes empty, gameplay unpauses.
  - `SettingsPanel` uses `OpenMenu(_mainPanel, false)` so its panel is treated as dependent of the opener.

---

## Audio system
- **Mixer**: `Assets/Audio/Resources/Master.mixer` with groups `Master`, `Music`, `Effects`, `Ambiance`, `Interface`, `Dialogue` (volumes are exposed and saved via `PlayerPrefs`).
- **Manager**: `AudioManager` lives in a hidden, `DontDestroyOnLoad` GameObject and loads:
  - Mixer + groups (by name),
  - `AudioDatabase` (`Resources/AudioDatabase`),
  - `audiomissing` fallback clip.
- **Volumes**: Use `AudioSettingsPanel` (sliders) or set via code:
```csharp
AudioManager.MasterVolume = 0.8f;      // 0..1 (internally saved as dB)
AudioManager.MusicVolume = 0.5f;
AudioManager.InterfaceVolume = 0.7f;
AudioManager.ResetVolumes();           // reapplies saved values to the mixer
```
- **Music**: Seamless crossfade with optional sync:
```csharp
AudioManager.PlayMusic(AudioClips.MyTrack, fadeTime: 1f, sync: true);
```
- **SFX**: Extension method on any `MonoBehaviour` (auto-routes to the right mixer group):
```csharp
this.PlaySound(AudioClips.ButtonClick);                     // uses registered SoundType
AudioManager.PlaySound(this, myClip, SoundType.Interface);   // explicit type
```
- **UI button sounds**: Add `ButtonAudioHandler` to your Button, assign `_clickClip` and `_hoverClip`.

### Adding audio clips (auto-enum + database)
Place clips under `Assets/Audio/<Category>/...` where `<Category>` is one of:
`Effects`, `Ambience`, `Music`, `Interface`, `Dialogue`.

On import/move/delete, the editor postprocessor:
- Updates `Assets/Scripts/Utilities/AudioClips.cs` (an enum of stable IDs) and
- Updates `AudioDatabase.asset` with the clip and its `SoundType` (inferred from the folder name).

If it doesn’t update (e.g., after a big VCS change), run: Tools > Fix AudioClips.

---

## Scriptable variables
- >**Note:** This section is inspired by the Unite talk ["Architecting Game Code with Scriptable Objects"](https://youtu.be/raQ3iHhE_Kk?t=926) by Ryan Hipple. For a deeper dive into the pattern and its benefits, see the [Unite 2017 presentation](https://youtu.be/raQ3iHhE_Kk?t=926).
- **Built-ins**: `BoolVariable`, `IntVariable`, `FloatVariable`, `StringVariable` (see `Assets/Scripts/Variables/`).
- **References**: `BoolReference`, `IntReference`, etc. In the inspector, click the dropdown to choose:
  - Use Constant (inline value),
  - Use Variable (reference an asset, generally a previously created asset under `Assets/Prefabs/Variables/Resources/`),
  - New Variable (will create an asset under `Assets/Prefabs/Variables/Resources/`).
- **At runtime**:
```csharp
public IntReference score;         // in inspector: constant or asset
void AddScore(int amount) {
    score.Value += amount;         // raises change events if using a variable
}
```
- **Reset behavior**: Variable assets can auto-reset to their default `Value` when a specific scene loads (set `Reset On Scene`).

---

## Event system
- **Create events**: Project window > Create > GameEvent. Or, in any inspector field for a `GameEvent`, click the plus button to create one (stored under `Assets/Prefabs/Events/Resources/`).
- **Listen to events**: Add `GameEventListner` to a GameObject, assign one or more events, then wire the `Response` UnityEvent. The listener passes a `DataVariable` wrapper to your callback.
- **Raise events**:
```csharp
// No payload
myEvent.Raise();

// With payload
var data = new EventData();
data.Data["Score"] = new EventDataElement("Score", intValue: 1000);
myEvent.Raise(data);

// Handler
public void OnGameEvent(DataVariable dv)
{
    // dv.Data may be null if the event was raised without a payload
    var score = dv?.Data?["Score"]?.IntValue ?? 0;
    var message = dv?.Data?["Message"]?.StringValue;
    Debug.Log($"Event received | Score: {score} | Message: {message}");
}

```
---

## Editor tooling
- **Scenes Window**: Window > Scenes Window — quick scene picker with per-directory filters (gear icon).
- **Batch Build**: Tools > Batch Build — builds WebGL, Windows, Mac, and Linux into the `Build/` folder.
- **Property drawers**: Better inspectors for scene fields (`SceneField`), game events, and variable references.

---

## Folder map (high level)
- `Assets/Audio/` — Organized by category; includes `Resources/Master.mixer` and `Resources/audiomissing.wav`.
- `Assets/Prefabs/` — `Game Manager.prefab`, UI prefabs (main menu, pause menu, settings parts).
- `Assets/Scenes/` — `MainMenu.unity`, `GameScene.unity`.
- `Assets/ScriptableObjects/Utilities/` — `GameData.asset`, `GameTime.asset`, `MenuStack.asset`, `AudioDatabase.asset`.
- `Assets/Scripts/` — Runtime, Variables, Utilities, and Editor scripts.

---

## Tips & troubleshooting
- **Pause doesn’t toggle**: Add a `Menu` button in Input Manager and bind Escape/Start.
- **Clips don’t appear in the enum**: Place them under `Assets/Audio/<Category>/...`, then run Tools > Fix AudioClips.
- **UI focus lost on controller**: Ensure an `EventSystem` exists; keep the default names `NewGameButton` and `ResumeButton` or set a selected object via script.
- **No sound**: Verify `Assets/Audio/Resources/Master.mixer` exists and groups match names in `AudioManager`.

---

## License
See `LICENSE` in the repo.
