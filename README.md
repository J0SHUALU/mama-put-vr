# Mama Put VR

You're running your aunty's Mama Put spot under a Lagos bridge for a few days. Scoop rice and stew, add the dodo and meat, hand the plate over, and count the right change in Naira before the queue gets vexed.

Built for Meta Quest 2 in Unity 6 with the XR Interaction Toolkit.

Gameplay video: [link]
Game Design Document: [link]

## What's in it

- Ladle and tongs that pick up portions from the pots and drop them onto plates when you tip them over
- A plate stack that always has a clean plate on top
- Customers who queue up, call out their order, and walk off if you take too long
- Payment in ₦100, ₦200, ₦500 and ₦1,000 notes, with change counted by hand from the change tray
- Five days with a target each, a chalkboard summary and up to three stars per day
- A short tutorial (Aunty's voice note) that waits for you to actually do each step
- Refill bowls on the back table for when a pot runs empty
- Seated mode and snap turn

## Requirements

- Unity 6 (6000.0 LTS or newer) with Android Build Support (SDK, NDK, OpenJDK)
- Packages: XR Interaction Toolkit 3.x, OpenXR Plugin, Meta OpenXR feature, Universal RP, TextMeshPro (comes with uGUI)
- A Meta Quest 2, 3 or 3S in developer mode, and a USB-C cable

## How to run it

1. Clone the repo and open the folder in Unity Hub with Unity 6.
2. Wait for the import. The first one takes a while.
3. Go to File > Build Profiles, pick Android and click Switch Platform.
4. Run **Mama Put > Apply Quest 2 Build Settings**. It sets IL2CPP, ARM64, ASTC, Vulkan and linear colour.
5. In Project Settings > XR Plug-in Management > Android, tick OpenXR, then add the Meta Quest feature group and the Oculus Touch controller profile.
6. Open `Assets/MamaPut/Scenes/MamaPut.unity`.
7. Plug in the headset, accept the USB debugging prompt inside it, and hit **Build And Run**.

To try it without a headset, open the scene and press Play with the XR Device Simulator from the XR Interaction Toolkit samples in the scene.

## How to test it

1. **Tutorial.** On first launch Aunty's voice note walks you through one full sale. It moves on only when you scoop, serve and get paid. Run **Mama Put > Reset Save Data** to see it again.
2. **Scooping.** Grab the ladle, dip it in the rice pot (a portion appears in it), hold it over a plate and tip it. The portion should land on the plate. The tongs only work on meat and dodo. The ladle only works on rice, jollof, stew and beans.
3. **Serving.** Read the order bubble, build the plate, and let go of it in front of the customer. A wrong plate gets a complaint and costs them patience. A right one moves them to paying.
4. **Paying.** Grab the note from their hand and drop it in the cash box. If they're owed change, take notes from the change tray and let go of them in front of the customer. Exact change counts towards the third star.
5. **Running out.** Keep serving until a pot is empty, then tip the matching refill bowl from the back table into it.
6. **Ending a day.** After the last customer the chalkboard shows sales, walkouts, exact change and stars. Hit the target and Next unlocks the following day.

## Scene setup

If you're rebuilding the scene from scratch, follow [docs/SCENE_SETUP.md](docs/SCENE_SETUP.md).

## Project layout

```
Assets/MamaPut/
  Scripts/
    Interfaces/   IUtensil, IFoodSource, IPlate, IPaymentHandler, ICustomerState and the rest
    Core/         DishData, DayConfig, Order, PlateContents, GameEvents, ObjectPool, Naira
    Interaction/  GrabbableItem, Utensil (Ladle, Tongs), FoodPot (pots), Plate, PlateStack, RefillBowl
    Money/        NairaNote, NoteDispenser, CustomerWallet, CashBox
    Customers/    Customer, the state classes, QueueManager, OrderGenerator
    Game/         DayManager, ScoreService, SaveSystem
    UI/           OrderBubble, PotTag, SummaryBoard, Tutorial, ComfortSettings
    Feedback/     FeedbackAudio, ItemSounds, Footsteps, AmbientOneShots
  Editor/         Menu items for default data, Quest settings and static batching
  Data/           Dish and Day assets
  Models/         Portion meshes, plus the danfo, keke and okada in Vehicles/
  Scenes/         MamaPut.unity
```

No concrete class inherits from another concrete class. Everything goes through an interface or an abstract base, and systems talk to each other through events on `GameEvents` instead of direct references. A new dish or day is a new ScriptableObject, made with **Mama Put > Create Default Dishes and Days** or by hand.

## Performance

The target is a steady 72 fps on Quest 2.

- Environment marked batching static (**Mama Put > Mark Selection Batching Static**)
- SRP Batcher on, with GPU instancing on customers, plates and notes
- LOD groups on the background crowd, danfos and buildings
- Kenney and KayKit models share one colour atlas per pack
- The danfo, kekes and okadas are under 400 polygons each, with a simpler danfo for its far LOD
- One warm mixed-mode sun with light probes for the moving customers
- IL2CPP, ARM64, ASTC, 4x MSAA, fixed foveated rendering
- Plates, notes, portions and customers are pooled, so nothing is created or destroyed mid-rush
- No occlusion culling: the stall is one open view and frustum culling already covers it

## Assets

- [Kenney Food Kit](https://kenney.nl/assets/food-kit) and [Mini Characters](https://kenney.nl/assets/mini-characters) (CC0)
- [KayKit Restaurant Bits](https://github.com/KayKit-Game-Assets/KayKit-Restaurant-Bits-1.0) and [City Builder Bits](https://github.com/KayKit-Game-Assets/KayKit-City-Builder-Bits-1.0) (CC0)
- Sound effects and music loop from [Kenney](https://kenney.nl/assets/category:Audio) (CC0)
- Street and market ambience from [BigSoundBank](https://bigsoundbank.com) (CC0)
- The danfo, keke and okada models, portion meshes, generator hum, danfo horns and all the UI are my own
