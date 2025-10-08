# Lab day 2

## Current Progress

### NavMesh

For navigation, we started experimenting with Unity’s built-in NavMesh system. <br>
The NavMesh allows us to define walkable surfaces within our scanned environment, so that an agent (in our case, the guiding arrow or path) can calculate the shortest route to a target location. <br>
We generated a NavMesh from the scan of the XR room, which gave us a test area where we could simulate navigation from one point to another. <br>
The automatically generated NavMesh was quite rough and contained several gaps, so instead, we recreated the floor mapping based on it and overlaid the two. This approach produced a much cleaner and more reliable NavMesh. <br>
This worked as expected in the simulated environment, but aligning the NavMesh with the real-world AR scan will present additional challenges, especially since furniture and obstacles may shift slightly compared to the static 3D model.

### GuidanceLine Asset

Additionally we added ed the "GuidanceLine" asset into our project to create dynamic visual paths for user navigation. The original asset created curved guidance lines using manually placed checkpoints, but we enhanced it to work automatically with our NavMesh system.

We created a new script `GuidanceLineAuto.cs` that:

- **Automatically calculates paths** using `NavMesh.CalculatePath()` instead of manual checkpoints
- **Generates smooth curved lines** that route around obstacles using the NavMesh data
- **Updates paths in real-time** when targets move or users deviate from the expected route
- **Optimizes performance** by only recalculating when necessary

### Testing

## Reflections

## Next steps

- UI for book ISBN/name input
- Routing and navigation to the book (arrow indicating direction and/or floor path)
- If needed: space recalibration with multi-anchor tracking

## Resources

---

_Written by: _

---
