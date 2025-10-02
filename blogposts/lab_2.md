# Lab day 2

## Current Progress

### NavMesh
For navigation, we started experimenting with Unity’s built-in NavMesh system. <br>
The NavMesh allows us to define walkable surfaces within our scanned environment, so that an agent (in our case, the guiding arrow or path) can calculate the shortest route to a target location. <br>
We generated a NavMesh from the scan of the XR room, which gave us a test area where we could simulate navigation from one point to another. <br>
The automatically generated NavMesh was quite rough and contained several gaps, so instead, we recreated the floor mapping based on it and overlaid the two. This approach produced a much cleaner and more reliable NavMesh. <br>
This worked as expected in the simulated environment, but aligning the NavMesh with the real-world AR scan will present additional challenges, especially since furniture and obstacles may shift slightly compared to the static 3D model.

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
