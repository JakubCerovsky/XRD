# Lab Day 1

## Introduction to our mobile AR project

For our first project, we decided to make a mobile AR application for VIA's library. <br>
Since libraries often have hundreds, if not thousands, of books, it can often be confusing to find the book that you are interested in. <br>
Our idea to solve this issue is to make a "find your book" system that allows you to first calibrate the environment by scanning an object or a picture, and search for a book either by ISBN or name. <br>
Using this indoor navigation powered by AR, users (students and librarians) could simply search for a book in the app and then follow a virtual path displayed through their phone or AR glasses. This reduces the time spent searching and increases accessibility, especially in this day and age when people often do not have any experience with physical libraries.

Additionally, a barcode scanner that reads the ISBN directly and/or integration with VIA systems could be added.

**In short**: Our system turns the traditional library experience into a smart, interactive, and user-friendly journey. This is achieved by using the immersive features the XR technologies provide, this not only makes the navigation faster but also enhance engagement and user satisfaction.

## Current Progress

### 3D scanning

For 3D scanning, we used a borrowed iPhone 14 Pro, that has a Lidar sensor, and used a [3D Scanner App](https://3dscannerapp.com/). <br>
We scanned a model of table 5 from the XR room (test environment) and a part of the library. For today, we used the one from the XR room to test alignment of the model with a (RHCP) patch as the origin.<br>
We exported these as .glb and .fbx files.
For working with GLB files in Unity, we used a package we imported by name: _com.unity.cloud.gltfast_ <br>
Later, after the lesson, we completed a full scan of the library.

### Blender

We used [Blender](https://www.blender.org/) to align the origin of the scan object (XR lab) to be the patch. We did this by going into edit mode, selecting the whole object and moving it so that the patch would be in the origin point.

### AR tracking

For AR tracking, we used [AR Tracked Image Manager component](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@6.0/manual/features/image-tracking.html). Key components for this are:

- An image of the patch and set the texture as a sprite
- An XR Reference Image Library with the photo inside, specifying the size of it in meters (in our case 0.08x0.08m)
- The scanned model of the room
- An AR Tracked Image Manager component in XR Origin, that uses the Reference Image Library as Serialized Library and the scanned model of the room as Tracked Image Prefab

### Testing

For quick testing, we went with a simple simulation environment that mimics the default XR Environment, but has the Simulated Tracked Image (the patch) on the table instead. <br>
Secondly, we built and ran the program on an Android phone and tested whether the model aligns properly with the real environment.

### Pre-built SDKs (MultiSet VPS, Immersal)

We tried to use pre-made solutions for indoor tracking that specialize in visual positioning systems (VPS) to potentially get better accuracy and reduce development complexity. <br>
Our initial focus was on MultiSet VPS and Immersal, as both solutions are designed for AR localization using spatial mapping.

For MultiSet VPS, we attempted to integrate their SDK into our Unity project. However, we encountered two main challenges. One being the immense size of the SDK in comparison to the other solutions. It would take about 10 minutes to compile on one of our teammate's laptop. The other being that their logo would appear on our app.

With Immersal, the integration process was more straightforward thanks to it being much smaller in code size. While promising, if it worked, we didn't manage to make it localize us in the room. Given more time, it could feasibly be debugged.

Although these pre-built SDKs could eventually help us achieve more accurate tracking and multi-anchor support, we decided to continue developing our own solution for now. This gives us more control over the process while allowing us to better understand AR tracking fundamentals. Later, we may revisit these SDKs to see if they can enhance or replace parts of our implementation.

### Scripting

Script writing and testing, mostly trying to replicate what AR Tracked Image Manager component is already achieving, but adding a delay.

### Version control

We are using Git with GitHub and Git LFS for version control.

Normally, Git is optimized for **text-based files** like code, but it struggles with **large binary files**. <br>
And Game development tends to have a lot of binary files, thus our choice of using it.

## Reflections

With most of us having close to no experience in Unity, this first lab day was a great opportunity to familiarize ourselves with the workflow, while also making some good progress on our project. <br>

## Next steps

- UI for book ISBN/name input
- Routing and navigation to the book (arrow indicating direction and/or floor path)
- If needed: space recalibration with multi-anchor tracking

## Resources

- [3D Scanner App](https://3dscannerapp.com/)
- [Blender](https://www.blender.org/)
- [AR Tracked Image Manager component](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@6.0/manual/features/image-tracking.html)

---

_Written by: Cosmin & Marcus_

---
