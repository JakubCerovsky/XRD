# Lab Day 6

## Introduction

During our last lab day we have focused on creating a main scene which served as a shared place for our individually implemented parts of the puzzle.

---

## Progress

### Task Division

**Marcus:** Integration of the Keypad Puzzle and Timer.

**Jakub:** Integration of the Shapes Puzzle, Adding audio source for the puzzle, playing with Near/Far Interactions

**Cosmin:** Putting puzzles together. Bomb levitation. Bomb Stop when defused. Adding sound: explosion, timer, keypad. Adding a flame below rocket.

**Alex:** Integration of the Wire Kit Puzzle, Adding audio to it, working with animation of the environment.

### Defuse Bomb Manager

New addition to our system was the `Defuse Bomb Manager` script serving as a parent for the individual puzzle scripts. This tracked whether the puzzle is solved or it failed to be solved. Based on given scenario the manager either defused the bomb and stopped the timer, or make the bomb explode.

### Audio Source

Another thing we added to the game was the audio sources. Giving the user feedback sounds, together with sounds relevant to the environemnt (drill, timer, explosion) makes the experience more immersive. This immersion is improved by the Unity's built-in 3D audio. This feature was enough for the scope of our project, as it is enough for now to only devide the sound between left/right side and near/fardistance. If sound would become more important in the future, we could use an audio spacializer plugin (such as OculusSpacializer) which would bring the HRTF (Head-related transfer function) in. HRTF would make direction of the sound more precise, distance would be more realistic, head movement and occlusion would come in play.

### User Interface

Adding a restart option in a form of a user interface, was our last addition to the system. We positioned it in the world to avoid motion sickness by placing static text that stays in front of the player's eyes and to not limit user interaction within the environment.

### OpenXR

With the help of the OpenXR plugin, we avoided the need for different configuration settings and headset-specific pipelines, which reduced the overall number of bugs and setbacks. This was achieved by allowing Unity to communicate with different devices through one common API provided by OpenXR. COOL...

In short, OpenXR replaces multiple VR pipelines with one clean, standardized build path.

### Video Demonstration

We have recorded a video gameplay from within the VR headset and the real-world palyer interactions with it.

The video can be found here: [VR Bomb Puzzle Demo](https://example.com)

## Reflections

The experience we have gained through the [AR Library Navigation System](https://github.com/JakubCerovsky/XRD) development was clearly shown in the last stage of this project. We have clearly separated the scenes for each of our individual parts, which helped drastically with the combination of our puzzles. There were only minor merge conflicts and the whole lab day worked out smoothly. Great success!

One thing we have noticed was how scattered the resources were across the folders. We have not set a specific structure of the folders, so the scripts, prefabs, and assets were all over the place. This should be improved in the future career prospects.

## Project Future

### Gaze Features

We have considered to add a gaze assistance to our game, which would track users eyes to assist with choosing the correct object to interact with. Unfortunately, the hardware we were working with (Meta Quest 1-3) did not have this feature available. This could improve the user experience drastically, as it would make the game interaction more smooth and straight forward.

### Near / Far Interaction

We have considered to remove the possibility for the player to grab/interact with object on far distance. This would make the whole game more realistic as the player would need to reach for each tool or shape physically with their hands, instead of interacting with them from across the room. We have deemed this change unnecassary as it would only increase the difficulty, but could definatelly come in play in future game improvements.

---

## Resources

- [OpenXR](https://www.khronos.org/OpenXR/)
- [XR Interaction Toolkit Examples](https://github.com/Unity-Technologies/XR-Interaction-Toolkit-Examples/blob/main/Documentation/index.md)
- [Correct & Wrong sound effects](https://www.youtube.com/watch?v=jZbaGCcdU48)
- [Explosion sound effect](https://www.youtube.com/watch?v=6Zla57jR6g4)
- [Bomb ticking sound effect](https://www.youtube.com/watch?v=dsEpxHP5L4c)
- [Key press and wire cut sound effects](https://www.youtube.com/watch?v=q8ZLBOFQ2g0)

---

_Written by: Jakub_

---
