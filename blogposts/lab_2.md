# Lab Day 2

During this lab session, our team focused on integrating the 3D-scanned model of the library into our AR navigation system. We worked on anchoring the scanned environment, exploring different navigation solutions, and experimenting with Unity’s NavMesh for pathfinding.

## Current Progress

The main two challenges the team faced this week were:

### 3D Scanning

We finished scanning the library with the borrowed iPhone and discussed which file type to use and how to process it. The whole scanning process took quite a lot of work since the library is a huge room and the application occasionally crashed.

### Anchoring the Scanned Environment Based on a Given Image

Based on the progress from last week, we used the completed scan and matched it with the [AR tracking](./lab_1.md/#ar-tracking) prepared during Lab 1.

### Navigate Around Library to Given Shelf

We debated whether to create our own navigation system or use an existing package. Considering the limited timespan for the project, we decided to use an existing asset. We researched multiple options, including:

- [SLAM](https://community.arm.com/arm-community-blogs/b/mobile-graphics-and-gaming-blog/posts/indoor-real-time-navigation-with-slam-on-your-mobile)
- [Unity NavMesh](https://learn.unity.com/tutorial/unity-navmesh#5c7f8528edbc2a002053b498)
- [Guidance Line](https://assetstore.unity.com/packages/tools/game-toolkits/guidance-line-303873)

#### SLAM (Simultaneous Localization and Mapping)

**SLAM (Simultaneous Localization and Mapping)** is a technology that allows a device to build a map of an unknown environment while tracking its own position within that map. It’s commonly used in robots, self-driving cars, and AR applications to help them understand and move through real-world spaces using sensors like cameras or LiDAR. In mobile AR (like ARKit or ARCore), which we would be using, SLAM runs in the background to estimate the user’s movement and orientation in real-time.

In our project, the library environment is already scanned and known, so we do not need to build a map from scratch like SLAM does.  
Instead, we only need a way to plan and display navigation paths inside this fixed 3D model. Hence... Unity NavMesh!

#### NavMesh

We still use AR tracking (which relies on SLAM) to know where the user is, but the navigation itself — the pathfinding and movement guidance — is handled by the **NavMesh**.  
For navigation, we started experimenting with Unity’s built-in NavMesh system.  
The NavMesh allows us to define walkable surfaces within our scanned environment so that an agent (in our case, the guiding arrow or path) can calculate the shortest route to a target location.  
We generated a NavMesh from the scan of the XR room, which gave us a test area where we could simulate navigation from one point to another.

The automatically generated NavMesh was quite rough and contained several gaps, so instead, we recreated the floor mapping based on it and overlaid the two. By setting these two parts as NavMesh Modifiers, we could easily decide how the NavMesh would “bake” these — and only these — objects, leaving out any other assets. This approach produced a much cleaner and more reliable NavMesh.  
This worked as expected in the simulated environment, but aligning the NavMesh with the real-world AR scan will present additional challenges, especially since furniture and obstacles may shift slightly compared to the static 3D model.

##### Player Object

We have started preparing the **Player Object**, which represents the initial position of the user simulated in the environment. The component used for this is the **NavMesh Agent**, which has the humanoid type and acts as a user in the simulation. Its parameters can be configured so the path maintains specific offsets within the environment.

#### Guidance Line

We took the time to research and test this shared asset with hopes that it would be exactly what we needed. However, we learned that this asset supports only hardcoded paths. While that would work nicely for a demonstration of the project idea, it would need to be replaced for a production-scale version of the project. What we decided to do was enhance it to work automatically with our NavMesh system.

We created a new script `GuidanceLineAuto.cs` that:

- **Automatically calculates paths** using `NavMesh.CalculatePath()` instead of manual checkpoints
- **Generates smooth curved lines** that route around obstacles using the NavMesh data
- **Updates paths in real-time** when targets move or users deviate from the expected route
- **Optimizes performance** by only recalculating when necessary

#### Destination Objects

We created two destination objects (**Shelf** and **Book**), which serve as targets for our navigation system. Later, we matched their transform details (position, rotation, and scale) to align with the real-life objects within our library scan.

### UI Changes

Additionally, we started preparing the initial interface the user will see upon opening the application. A simple prompt explaining the basics of the app was implemented and will be enhanced with an ISBN number input field in the next lab class.

## Reflections

Overall, we performed a solid lab day and definitely achieved more than during the previous one. We all agreed that tasks which took us hours to implement before would now take us much less time. However, there is still a lot to implement, and we can only hope that this tight project timespan will be enough for us to deliver a solid result.

Furthermore, after today’s talk with Kasper, we became aware that the main focus of our project should heavily revolve around the AR features, as they are the most relevant for this course.

We also gained a deeper understanding of how AR tracking depends on stable anchoring and how NavMesh generation can be optimized for performance.

## Next Steps

- UI for book ISBN/name input
- Routing and navigation to the book (arrow indicating direction and/or floor path)
- Start navigation after inputting a given ISBN number
- If needed: space recalibration with multi-anchor tracking

## Resources

- [Unity NavMesh](https://learn.unity.com/tutorial/unity-navmesh)
- [Blender](https://www.blender.org/)

---

_Written by: Jakub & Markus_

---
