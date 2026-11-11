# Fish

Mac-first 3D fishing game. This branch is the first playable slice: a Unity 6 URP project and a third-person walk around a greybox mountain pond. Fishing, fish, the journal, boats, and the Link's Awakening look are not in this slice.

这是第一段可玩内容：用 Unity 6 URP 在灰盒山间水塘边做第三人称走动。钓鱼、鱼、手帐、船，以及《织梦岛》风格，都不在这一段里。

## Open and press Play（打开并运行）

Tested target: **Unity 6000.3.28f1** (Unity 6.3 LTS), Apple Silicon macOS. This project was scaffolded from Unity's URP blank settings; the cloud machine does not have the Unity Editor, so the first open on your Mac is the real import.

1. Install [Unity Hub](https://unity.com/download).
2. Install editor **6000.3.28f1** (Unity 6.3 LTS). In Hub, choose the **Apple silicon** build, not Intel-only.
3. Modules when you install that editor:
   - **Mac Build Support (Mono)** — not required to press Play. Install it if you want to build a `.app` later.
   - **Mac Build Support (IL2CPP)** — not required for this slice.
   - Documentation, Android, iOS, and Windows modules — not required.
   - Visual Studio for Mac is discontinued. Use Rider, Visual Studio Code, or another editor. The project includes the Rider and Visual Studio Editor packages so `.csproj` files generate.
4. Hub → **Open** → select this repository folder (the one that contains `Assets`, `Packages`, and `ProjectSettings`).
5. First import downloads packages (URP 17.3, Input System, Cinemachine, Splines) and can take a few minutes. If Hub offers to enable the new Input System or to realign a core package version to this editor, accept it and let the editor restart. Active Input Handling is already **Input System Package**.
6. The editor script opens `Assets/Scenes/MountainPond.unity` when the project starts on an empty untitled scene. If you only see an empty sky, double-click that scene in the Project window.
7. Press **Play**. Click the **Game** view so the cursor locks.

### Controls

| Input | Action |
| --- | --- |
| W A S D or arrows | Walk, camera-relative |
| Mouse | Look (yaws the body, pitches the shoulder camera) |
| Left Shift | Jog (walk is the default) |
| Esc | Free the cursor |
| Click the Game view | Lock the cursor again |

There is no jump and no climb. The CharacterController step offset is 0.34 m and the slope limit is 50°, so the low rocks and the outcrop stairs/ramp are walkable. Steeper mountain faces are not. The pond has no swimming: falling off the dock or the bank sends you back to camp.

操作：WASD 或方向键行走，鼠标环顾，按住左 Shift 慢跑，Esc 释放鼠标，再点 Game 视图重新锁定。不能跳、不能攀爬。矮石和观景岩的台阶、缓坡可以走上去；更陡的山体走不上去。还不能游泳，从码头或岸边掉进水里会回到营地。

## What is in the scene

`Assets/Scenes/MountainPond.unity` — a basin about 60 m across:

- **Grassy bank** on the south shore, where you spawn.
- **Camp** (tent, fire, log) and a short path of stones toward the water.
- **Wooden dock** on the west shore, a few meters out over the water.
- **Rocky outcrop** on the east shore, a little higher, with stairs (step offset) and a ramp (slope).
- **Reed shallows** in the northwest of the pond (sandbar and reeds).
- **Deeper end** on the north, a darker water plane.
- Mountains around the apron. A steep north foothill is intentionally too steep to walk up.

The character is a capsule placeholder. `Angler/RodSocket` is an empty transform for a rod later. Greybox materials are URP Lit. Water is a plane. Lighting is a single warm sun plus the campfire, not the final early-morning grade.

## Project layout

```
Assets/Input/          Input System actions (Player/Move, Look, Sprint)
Assets/Materials/      Greybox URP Lit materials
Assets/Scenes/         MountainPond.unity
Assets/Scripts/Player  AnglerMotor
Assets/Scripts/Camera  ShoulderCameraRig (Cinemachine Third Person Follow)
Assets/Scripts/UI      Play-mode control hint
Assets/Scripts/Editor  Opens the pond scene on a fresh editor load
Assets/Settings/       URP pipeline, renderer, volume profile
```

Packages: Universal RP, Input System, Cinemachine, Splines (Cinemachine dependency), uGUI. The shoulder camera is created in Play mode so the scene does not hand-serialize Cinemachine components.

Suggested next folders, not created yet: `Assets/Scripts/Fishing`, `Assets/Art`.

## If something looks wrong on first open

- **Pink meshes:** Universal RP did not finish importing. Window → Package Manager, confirm Universal RP is installed, then reimport `Assets/Materials`.
- **No input:** Project Settings → Player → Active Input Handling = Input System Package, then restart the editor.
- **Empty Game view:** open `Assets/Scenes/MountainPond.unity` and press Play again.
- **Mouse does nothing:** click the Game view. Esc unlocks the cursor on purpose.
