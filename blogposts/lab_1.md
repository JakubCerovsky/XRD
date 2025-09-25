# Lab day 1

## Introduction to our mobile AR project

For our first project, we decided to make a mobile AR application for VIA's library. <br>
Since libraries often have hundreds, if not thousands of books, it can often be confusing to find the book that you are interested in. <br>
Our idea to solve this issue is to make a "find your book" system that allows you to first calibrate the environment by scanning an object or a picture and search for a book either by isbn or name. <br>
This could also be useful for the librarian if they want to know where to place a certain book. A qr/barcode scanner that fetches the isbn directly and/or integration with VIA systems could be added.

## Current Progress

### 3D scanning
For 3D scanning we used a borrowed iPhone 14 Pro, that has a Lidar sensor and used a [3D Scanner App](https://3dscannerapp.com/). <br>
We scanned a model of table 5 from the XR room (test environment) and one of a part of the library. For today, we used the one from the XR room to test allignment of the model with a (RHCP) patch as the origin.<br>
We exported these as .glb and .fbx files.
For working with glb files in Unity, we used a package we imported by name: _com.unity.cloud.gltfast_

### Blender

We used [Blender](https://www.blender.org/) to allign the origin of the scan object (XR lab) to be the patch. We did this by going into edit mode, selecting the whole object and moving it so that the patch would be in the origin point.

### AR tracking

For AR tracking, we used [AR Tracked Image Manager component](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@6.0/manual/features/image-tracking.html). Key components for this are:
- An image of the patch and set the texture as a sprite
- An XR Reference Image Library with the photo inside, specifying the size of it in meters (in our case 0.08x0.08m)
- The scanned model of the room
- An AR Tracked Image Manager component in XR Origin, that uses the Reference Image Library as Serialized Library and the scanned model of the room as Tracked Image Prefab

### Testing

For quick testing, we went with a simple simulation environment that mimics the default XR Environment, but has the Simulated Tracked Image (the patch) on the table instead. <br>
Secondly, we built and ran the program on an android phone and tested if the model alligns properly with the real environment.

### Pre-built packages

We tried to use pre-built packages for indoor tracking. 

### Scripting

Script writing and testing, mostly trying to replicate what AR Tracked Image Manager component is already achieving but adding a delay.

### Version control

We are using Git with GitHub and Git LFS for version control.

## Reflections 

With most of us having close to no experience in Unity, this first lab day was a great oportunity to familiarize ourselves with the workflow, while also making some good progress on our project. <br>

## Next steps

- UI for book isbn/name input
- Routing and navigation to the book (arrow indicating direction and/or floor path)
- If needed: space recalibration with multi-anchor tracking

## Resources

- [3D Scanner App](https://3dscannerapp.com/)
- [Blender](https://www.blender.org/)
- [AR Tracked Image Manager component](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@6.0/manual/features/image-tracking.html)

---

_Written by: Cosmin_

---