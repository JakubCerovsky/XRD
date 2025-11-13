# Lab Day 4

## Introduction

During Lab Day 4, we shifted our focus to planning and dividing tasks for our new VR bomb-defusal project. The goal was to implement the main environment, create the first puzzle mechanics, and begin developing the tools and interactions that will make the game engaging.

Before starting, we discussed the concept and drew a diagram to visualize the experience, how puzzles will connect, and what interactions each player will have. Based on this discussion, we divided responsibilities to ensure everyone could make steady progress in parallel.

> **Project Structure Diagram:** overview of puzzle flow, environment layout, and assigned responsibilities.  
![Project Structure Diagram](./VR_Bomb_Puzzle_Flow.png)

## Progress

### Task Division

After reviewing the diagram and discussing possible mechanics, we split our tasks as follows:

**Marcus:** Keypad puzzle; implemented the interaction logic, made the scripting of the keypad input system, and created a working keypad prefab that works with ray interaction.

**Jakub:** Shapes puzzle (It goes in the square hole); used ProBuilder to design and implement the shape-fitting puzzle, focusing on object interaction with XR Grab Interactable, colliders, and physics components.

**Cosmin:** Environment setup — created the base VR environment, added teleportation points, imported and placed the bomb model, and ensured the scene was connected properly to the headset for testing.

**Alex:** Tool system — researched and imported VR tool assets, including screwdrivers and other components, and started developing the screwing mechanism that will later be used to open panels on the bomb.

We also agreed that the timer system would be part of the bomb model itself. For now (week 1), the timer asset was only thought considered. In week 2, we plan to find an asset and display the timer using a floating screen to make it easier to test and debug without full bomb integration.

### Keypad Puzzle

The **keypad model** was sourced from a YouTube tutorial. Although the creator did not originally provide a download link, after some searching and directly contacting them, they were kind enough to share the model for use in our project.

The **display screen** uses **TextMeshPro** to render the entered numbers, ensuring sharp and readable text.  
Each **button** was individually configured with an interactivity component to make it responsive to user input within the VR environment.
The scripting process went smoothly overall. At this stage, the keypad correctly registers user input and displays it on the screen. The only remaining part to implement is the logic for handling correct and incorrect codes.

### Shapes Puzzle

The first thing was to set up the objects needed for this part of the puzzle.

#### Creating 3D Objects

**ProBuilder** package was used for creating Shapes objects for on of the puzzles. With the help of an experimental tool, it was simple to "subtract" individual shapes from one object, to easily create a board where each of them perfectly fits.

Unfortunately, these ProBuilder objects were not rendering in the environment while using XR Grab Interactable, and were exported as assets and later on used as prefabs. As a result, there were 5 prefabs:

- 1 Board serving as a target for placing each shape object
- 4 different shapes aimed to be placed on the board

_NOTE: These prefabs can be found in `Assets/VRTemplateAssets/Prefabs/ShapesPuzzle`._

#### Grabbable Object

**XR Grab Interactable** component was used on each of the shapes so the user can interact with them (grab, rotate, etc.) and place them inside the board.

**Rigidbody** was used on each of shapes object simultaneously as it is per default required while using the **XR Grab Interactable**. This component enables the game object to act based on physics.

Additionally, to enforce a more specific physics behaviour **Mesh Colliders** (and other colliders) and **Rigidbody** were used on each of the shapes. Using these two together helped with differentiating whether the object is dynamic, kinematic, or static. Since the board is only a static object, the only component required there was the **Mesh Collider**. The shapes were set as dynamic, as they are to use gravity, and are not meant to go through the static objects. For this to happen, a Velocity Tracking Movement Type was used in the XR Grab Interactable component.

Lastly, to improve the user experience, shapes use the dynamic attachment, so the user can grab from various angles.

### Environment

For the environment, we initially considered building a custom space from scratch. However, after some exploration, we found a high-quality **VR-ready room model on Sketchfab** that already included **baked lighting**, which made it visually appealing and performance-friendly.  
We decided to use this model as our base environment since it honestly looked pretty nice.

At first, the environment allowed the player to **teleport anywhere**, including outside the intended play area by teleporting and clipping in the walls (spooky).  
To fix this, Cosmin created a **NavMesh** that defined the walkable surfaces within the scene. This ensured that teleportation would only work inside valid areas.

The **teleportation system** itself was imported from the **Unity VR Template**, which provided a solid and easy-to-customize base for player movement and interaction.

Also, Cosmin started creating the bomb model in Blender, which will be a floating box flying out of the cardboard box.

### Tool system puzzle 

For the workplace tools, **Alex** was responsible for finding, importing, and setting up the necessary 3D models.  
A collection of realistic **tool assets** — including screwdrivers, pliers, and other hand tools — was added to the project to enhance the immersion of the bomb defusal environment.

These tools will then be prepared for future interaction scripts, such as the **screwing and unscrewing mechanism**, which will later allow players to open bomb panels and interact with the internal wires that will need to be cut.

## Reflections

This lab day was mainly focused on organization and early implementation.
It was productive because everyone clearly understood their role and could work independently while still contributing to the same goal. Having the diagram helped us visualize the final structure and avoid overlapping work.

Even though the bomb defusal experience is still in its early stages, we now have:
A functional environment connected to the VR headset
The first puzzles (Shapes puzzle; Keypad puzzle) were created but not connected
The start of tool interactions
An early bomb model with teleportation ready for testing

Next week, we will continue by integrating the floating timer display, refining the puzzles, and testing.

## Resources

- [VR Keypad tutorial](https://www.youtube.com/watch?v=H-FPfxr-hKY)
- [Unity Physics: Static, Kinematic, Dynamic](https://www.youtube.com/watch?v=xp37Hz1t1Q8)
- [VR Development](https://learn.unity.com/pathway/vr-development?version=6.2)
- [ProBuilder](https://docs.unity3d.com/Packages/com.unity.probuilder@6.0/manual/index.html)
- [Enviroment Model](https://sketchfab.com/3d-models/rock-wall-basement-with-baked-lighting-71912d43c4724e3ab12d9bf2be80f728)
- [Workplace Tools](https://assetstore.unity.com/packages/3d/props/industrial/workplace-tools-86242)

---

_Written by: Marcus_

---
