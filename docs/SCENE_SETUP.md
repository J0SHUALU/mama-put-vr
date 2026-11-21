# Scene setup

How `MamaPut.unity` is put together, so it can be rebuilt or fixed. Every object that the player grabs needs an **XR Grab Interactable**, a **Rigidbody** and a collider. Every zone below is a collider with **Is Trigger** ticked.

## 1. Project and rig

1. New project from the Unity 6 **VR** template (it comes with XRI, OpenXR and URP set up).
2. Delete the sample scene contents except the **XR Origin (XR Rig)**. Put it behind the counter, about 0.4 m from the edge.
3. On the XR Origin, turn off continuous move and keep snap turn only.
4. Run **Mama Put > Create Default Dishes and Days**. It fills `Assets/MamaPut/Data`.
5. Import the Kenney Food Kit, Kenney Mini Characters and KayKit Restaurant Bits into `Assets/ThirdParty`.

## 2. Portions

1. Drag each mesh from `Assets/MamaPut/Models` into the scene, give it a URP Simple Lit material in the right colour (orange jollof, off-white rice, dark red stew, golden dodo) and save it as a prefab in `Assets/MamaPut/Prefabs/Portions`.
2. Use the Kenney `meat-cooked` model for the meat portion.
3. Remove colliders from portion prefabs. They are visual only.
4. Open each Dish asset and drag in its portion prefab.

## 3. Stall

| Object | Model | Components |
|---|---|---|
| Counter | KayKit `kitchentable_B_large` | Box collider. Mark batching static |
| Rice pot, Jollof pot | KayKit `pot_large` | **RicePot**, trigger collider over the opening, Dish set |
| Stew pot | KayKit `pot_A_stew` | **StewPot**, trigger, Dish = Stew |
| Dodo tray | Kenney `plate-deep` | **DodoTray**, trigger, Dish = Dodo |
| Meat bowl | KayKit `bowl` | **MeatBowl**, trigger, Dish = Meat |
| Back table | KayKit `kitchentable_A` | Box collider. Static |
| Refill bowls (one per dish) | KayKit `bowl` | **RefillBowl**, grab interactable, Rigidbody, collider, Dish set |

For **Fill Visual** on a pot, drop a flat disc of the dish colour just under the rim and drag it in. It sinks as the pot empties.

## 4. Utensils and plates

1. **Ladle**: Kenney `cooking-spoon`, add **Ladle**, an XR Grab Interactable, a Rigidbody, a capsule collider for the handle, and a small sphere trigger on the bowl end. Add an empty child at the bowl called `LoadPoint` and drag it into **Load Point**.
2. **Tongs**: same setup with **Tongs**.
3. **Plate prefab**: Kenney `plate`, add **Plate**, XR Grab Interactable, Rigidbody, a box collider and a box trigger above it. Add 4 empty children as slots spread across the plate and drag them into **Slots**. Tick GPU instancing on its material.
4. **Plate stack**: empty object on the counter with **PlateStack**, Plate Prefab set and a `SpawnPoint` child.

## 5. Money

1. Make four note prefabs (₦100, ₦200, ₦500, ₦1,000) from a thin box with the note texture from the atlas. Add **NairaNote** with the right Value, XR Grab Interactable, Rigidbody and a box collider.
2. **Cash box**: a tin (KayKit `jar_A_large`) with **CashBox** and a trigger over the opening.
3. **Change tray**: four empty objects with **NoteDispenser**, one per note prefab, each with a `SpawnPoint` child.
4. **Wallet**: empty object with **CustomerWallet**, all four note prefabs in Note Prefabs.

## 6. Customers

1. For each Kenney mini character: add **Customer**, an Animator with a `Speed` float if it has walk animations, and an empty `Hand` child in front of the chest.
2. Add a child called `HandZone`: box trigger roughly 0.5 m wide in front of the customer, with **CustomerHandZone** pointing back at the Customer.
3. Add the order bubble: a world-space Canvas above the head with a background Image, a TMP text and a radial Image for patience (Image Type Filled). Put **OrderBubble** on it and drag them in. Drag the bubble into the Customer's **Bubble** field.
4. Save each one as a prefab.
5. **Queue**: empty object with **QueueManager**. Add 5 empty spots in a line leading away from the counter (spot 0 at the counter) and a `StallPoint` on the counter.
6. Add `SpawnPoint` and `ExitPoint` empties off to the side, out of view.

## 7. Game manager

Empty object called `Game` with:

- **OrderGenerator**
- **ScoreService**
- **DayManager**: the five Day assets in order, the customer prefabs, spawn and exit points, Queue, Cash Box, and drag the OrderGenerator and ScoreService components into their slots
- **FeedbackAudio**: an AudioSource and the clips

## 8. Board, tutorial and comfort

1. **Summary board**: a world-space canvas on the stall post, styled as chalkboard. Add title and body TMP texts and three Buttons (Start, Next, Retry) wired to **PressStart**, **PressNext**, **PressRetry**. Add a **Tracked Device Graphic Raycaster** to the canvas so poke and ray work.
2. **Tutorial**: another small world-space canvas with a TMP text, an AudioSource and a Skip button wired to **Skip**. Fill in 4 steps: scoop, serve, take payment and give change, done.
3. **Comfort**: **ComfortSettings** on the XR Origin, with a button on the board wired to **ToggleSeated**.

## 9. Environment and performance

1. Street, bridge, buildings and the stall frame: select them and run **Mama Put > Mark Selection Batching Static**.
2. Background crowd, parked cars and far buildings: add an **LOD Group** with LOD0, LOD1, LOD2 and Culled. Make the lower meshes with Blender's Decimate modifier.
3. One directional light set to Baked. Add a Light Probe Group around the stall. Bake from Window > Rendering > Lighting.
4. On the URP asset: MSAA 4x, HDR off, main light shadows off, additional lights off.
5. Fixed foveated rendering: in OpenXR Meta settings, set it to High.
