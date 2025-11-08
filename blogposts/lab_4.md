# Lab Day 4

## Introduction

---

## Progress

## Shapes Puzzle

First thing was to setup the objects needed for this part of the puzzle.

### Creating 3D Objects

**ProBuilder** package was used for creating Shapes objects for on of the puzzles. With the help of experimental tool, it was simple to "subtract" individual shapes from one object, to easily create a board where each of them perfectly fits.

Unfortunately, these ProBuilder objects were not rendering in the environment while using XR Grab Interactable, and were exported as assets and later on used as prefabs. As a result there were 5 prefabs:

- 1 Board serving as a target for placing each shape object
- 4 different shapes aimed to be placed in the board

_NOTE: These prefabs can be found in `Assets/VRTemplateAssets/Prefabs/ShapesPuzzle`._

### Grabbable Object

**XR Grab Interactable** component was used on each of the shapes so the user can interact with them (grab, rotate, etc.) and place them inside the board.

**Rigidbody** was used on each of shapes object simultaneously as it is per default required while using the **XR Grab Interactable**. This component enables the game object to act based on physics.

Additionally, to enforce a more specific physics behaviour **Mesh Colliders** (and other colliders) and **Rigidbody** were used on each of the shapes. Using these two together helped with differentiating whether the object is dynamic, kinematic, or static. Since the board in only a static object, the only component required there was the **Mesh Collider**. The shapes were set as dynamic, as they are to use gravity, and are not meant to go through the static objects. For this to happen a Velocity Tracking Movement Type was used in the XR Grab Interactable component.

Lastly, to improve the user experience, shapes use the dynamic attachment, so the user can grab from various angles.

---

## Reflections

---

## Resources

- [Unity Physics: Static, Kinematic, Dynamic](https://www.youtube.com/watch?v=xp37Hz1t1Q8)
- [VR Development](https://learn.unity.com/pathway/vr-development?version=6.2)
- [ProBuilder](https://docs.unity3d.com/Packages/com.unity.probuilder@6.0/manual/index.html

---

_Written by: NAME_

---
