# Mama Put VR

You're running your aunty's Mama Put spot under a Lagos bridge for a few days. Scoop rice, stew, beans and egusi, add the dodo, hand the plate over, and count the right change in Naira before the queue gets vexed.

Built for Meta Quest 2 in Unity 6 with the XR Interaction Toolkit.

Gameplay video: [link]
Game Design Document: [link]

## What's in it

- A main menu with the controls, plus Settings (snap or smooth turn, seated mode, volume) and Quit
- A steel ladle and tongs that pick up portions from the pots and drop them onto plates when you tip them over. Dip a full spoon back in its pot to put the food back
- Six dishes (Jollof, White Rice, Beans, Stew, Egusi and Dodo) heaped in aluminium pots, with dodo in an enamel bowl. Every portion and pot is textured from a real photo of the dish
- A stack of white enamel plates with a blue rim, always with a clean one on top. The ladle, tongs and plates sit between you and the pots
- Customers who queue up along the pavement, show their order in a bubble ("2 Dodo"), carry the plate off in both hands, and walk off if you take too long
- Payment in real ₦100, ₦200, ₦500 and ₦1,000 note designs, with change counted by hand from a cash table at your right hand
- A till readout over the stall with today's takings and any change you still owe
- Five days that get harder: one dish and exact money on Day 1, up to three dishes, change and less patience by Day 5
- A results screen after every day with your stats, tips when something went wrong (or "Well done" when nothing did), and Retry, Continue or Main menu
- A short tutorial card from Aunty Funmi, just over the counter, that waits for you to actually do each step. It runs each time you press Play and has a Skip button
- Refill bowls with name cards on the back table for when a pot runs empty
- Pause from the menu button on the left controller or the Pause button by the till
- Moving danfos, kekes and cars, a recorded Lagos bus stop in the background and food-specific scoop sounds

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

## Controls

| Input | What it does |
|---|---|
| Left stick | Walk |
| Right stick | Turn (smooth by default, snap in Settings) |
| Trigger or grip | Grab. Point at something and pull to bring it to your hand. Let go to drop |
| Tip the spoon | Serve the portion onto the plate under it |
| A or X | Hand over: serves the plate you're holding, gives the change you're holding, or takes the customer's money if your hand is empty |
| Point + trigger | Press menu buttons |
| Menu button (left controller) | Pause |

## How to test it

1. **Tutorial.** After pressing Play, Aunty's card over the counter walks you through one full sale. It moves on only when you scoop, serve and get paid. Press Skip to go straight to the day.
2. **Scooping.** Grab the ladle, dip it in the rice pot (a portion appears in it), hold it over a plate and tip it. The portion should land on the plate. Either utensil works on any dish. Dip a full spoon back into the same pot to put the food back.
3. **Serving.** Read the order bubble, build the plate with exactly what it says, and press A while holding the plate (or let go of it in front of the customer). A wrong plate gets a complaint that says what was wrong and costs them patience. A right one moves them to paying.
4. **Paying.** Press A with an empty hand to take their money, or drop their note in the cash tin. If the till says change is owed, grab notes from the change tray and press A. Exact change counts towards the third star.
5. **Running out.** Keep serving until a pot is empty, then tip the matching refill bowl from the back table into it.
6. **Ending a day.** After the last customer a results screen shows sales, walkouts, exact change, stars and tips. Continue opens the next day whether or not you hit the target. The target is what earns the extra star.

## Scene setup

If you're rebuilding the scene from scratch, follow [docs/SCENE_SETUP.md](docs/SCENE_SETUP.md).

## Project layout

```
Assets/MamaPut/
  Scripts/
    Interfaces/   IUtensil, IFoodSource, IPlate, IPaymentHandler, ICustomerState and the rest
    Core/         DishData, DayConfig, Order, PlateContents, GameEvents, ObjectPool, Naira
    Interaction/  GrabbableItem, Utensil (Ladle, Tongs), FoodPot (pots), Plate, PlateStack, RefillBowl, ControllerHand, HandOverButton
    Money/        NairaNote, NoteDispenser, CustomerWallet, CashBox
    Customers/    Customer, the state classes, QueueManager, OrderGenerator, CustomerPlateHold
    Game/         DayManager, ScoreService, SaveSystem, VehicleDrive
    UI/           MainMenu, SettingsPanel, PauseMenu, ResultsPanel, TillCounter, OrderBubble, PotTag, SummaryBoard, Tutorial, ComfortSettings
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

- [Kenney Food Kit](https://kenney.nl/assets/food-kit) (CC0)
- Customers and their idle and walk animations from [Mixamo](https://www.mixamo.com), cut down to about 10k polygons and 512px textures each
- [KayKit Restaurant Bits](https://github.com/KayKit-Game-Assets/KayKit-Restaurant-Bits-1.0) and [City Builder Bits](https://github.com/KayKit-Game-Assets/KayKit-City-Builder-Bits-1.0) (CC0)
- Sound effects and music loop from [Kenney](https://kenney.nl/assets/category:Audio) (CC0)
- Street and market ambience from [BigSoundBank](https://bigsoundbank.com) (CC0)
- Lagos bus stop ambience ("Ambient Saboyaba Busstop Lagos Nigeria") by freesound_community on [Pixabay](https://pixabay.com/sound-effects/) (Pixabay Content License)
- Scoop sounds cut from "Coffeebean Scoop" and "Soup Dripping Into Pot" by freesound_community on [Pixabay](https://pixabay.com/sound-effects/) (Pixabay Content License). The grainy one is used for the rice dishes and dodo, the wet one for stew, beans and egusi
- Food textures cropped from reference photos of each dish (the white rice photo is from [Vecteezy](https://www.vecteezy.com))
- [Ojuju](https://fonts.google.com/specimen/Ojuju) font by the Ojuju Project Authors (SIL Open Font License)
- Naira note images from the [Central Bank of Nigeria currency gallery](https://www.cbn.gov.ng/currency/gallery/) (official specimen images, kept with a faint SPECIMEN mark)
- The danfo, keke and okada models, food meshes, pots, enamel bowl and plates, ladle, tongs, generator hum, danfo horns and all the UI are my own
