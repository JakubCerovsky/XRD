# Lab Day 3

During the last AR Lab Day, we started putting our individual scenes and assets together, so we have a functional prototype of the application.

---

## Progress

### NavMesh + Guidance Line

Our results of the second lab day were that we had a NavMesh Surface (walkable surface) of the library scan, and we had a GuidanceLineAuto asset working with a sample scene intergrated with NavMesh. Our first step was to use this guidance line with our custom setup for the library.

This was a pretty straight forward task, and in fact, in immediately worked in the [XR Simulation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.0/manual/xr-simulation/simulation.html), which allows us to test the AR application. Unfortunately, things were going too smoothly for us and soon we encoutered a problem. The issue we uncovered was that when we tried to build the app in our mobile device, the basic pathfinding was indeed successful, however, the path did not update based on the user/mobile camera movements. This was something we have spent the most of our Lab Day 3; change script, rebuild, repeat. Nothing seemed to fix our issue so we decided to move onto more important things and leave this for the [project future](#project-future)

As a last step, we made the scanned environment transparent, so it does not affect the user interaction with the real-world view. Keeping the environment in helps with **occlusion** as the navigation line is not displayed behind the shelfs.

### Search Book by ISBN Number

Our main improvement for this Lab Day was improving the `GuidanceLineAuto` script and introducing the `BookDatabase` script. There are several steps it does during the runtime:

1. User inputs the ISBN number
   - The `BookDatabase` script checks if the ISBN format is valid and the book is in the system
2. `GuidanceLineAuto` script sets the main camera as the start point of the navigation.
3. `GuidanceLineAuto` script sets the book as the end point for the navigation.
4. `GuidanceLineAuto` script automatically generates a line, which traces a path between the start point and the end point.
5. _Optional: User can leave the navigation._

### ISBN Format Checker

Checks the fortmat of the inputed ISBN value whether it is one of the valid patterns based on the [ISBN documentation](https://www.isbn.org/faqs_general_questions).

### UI

Final UI components were implemented with the following features:

- Introducing the application
- Requesting the user to input desired ISBN number
  - This feature is using the ISBN Format Checker mentioned above
- Option to "Exit" the navigation process

### Setback

At the end of the Lab 3, we have found out that some of the last three pull-requests broke our AR experience on mobile device. While everything was working smoothly in the XR Simulation, we have forgot to test the latest changes on our phones. The issue was that the camera on the phone did not work as expected after opening built application.

After spending quite a lot of time debugging and comparing commits, we were unable to find the root of a problem. At the end, We decided to revert to latest working commit, and implement the missing features again.

This included:

- [Search Book by ISBN Number](#search-book-by-isbn-number)
- [UI Changes](#ui)

Knowing what to do, these changes were failry simple, but still took us time, since we were testing the build on phone after each small step. This setback was rather unfortunate, and dealt a solid damage to out progress. However, we have learned the importance of testing each step, and not to trust XR Simulation.

### Video Demonstration

We have prepared a video scenario, and recorded the demonstration in the school library with one of the books faked in the system.

The video can be found here: [AR Library Navigation Demo](https://youtu.be/h0o3pto08v4)

---

### Project Future

There were imperfections we were aware of, however, the timespan limited us to deliver basically a minimal viable product. Therefore, there are things we would desire to work on in future of this project.

#### Improvements

- **Navigation Path**
  As mentioned before, the navigation path is not updating in the mobile application. This most likely requires debugging of the current approach and should not pose as too big of a challange for more experienced Unity developers.

- **Markerless Tracking**
  Users could open the app anywhere in the library, and the AR system would automatically recognize the environment based on cloud-anchors and correctly place the navigation overlay without the need for a specific image marker (like a poster we use now) to initialize the AR experience.

- **NavMesh Surface**
  Currently, the scan is not perfect, so the NavMesh Surface has its flaws which could be improved. For example, certain objects (table, pot, etc.) within the library are marked as walkable. This creates a confution in the application and should be addressed.

- **Verticle Poster Position**
  Our inital testing only involved anchoring the environment based on a table-top positioned poster, and we did not acount for challanges we had with putting the poster onto a wall. This changed the rotation of our environment, and we could not figure out a way to solved this, and resorted to use the horizontal position.

#### Scaling

- **Integrate Library Database**
- **Navigate to shelf**
- **Include Barcode Scanner**
  Take the ISBN number from the scanned barcode. This could be nice for the librarians returning the book.

---

### Reflections

Putting things together from our seperate scenes was like using a building blocks to finalize a product. It was satisfing to see the work we have done over the last weeks being put together, as the user could actually navigate within the library to a book by the ISBN. Nevertheless, there were still things we needed to implement, and the confution from poor communication, and sudden work in the same scenes and on the same assets created a multiple merge conflits. This was a bit unproffesional from us, and perhaps (we really do not know), could have been resolved easily with Unity Version Control.

#### Overall Reflection

There were several challenges we have faced over the whole AR development, which slowed us down, but also made us learn. Considering that most of us went into this project with close to no experience with Unity, and AR Development, this turned into a pretty well-functioning AR project. The size of the project might have been too large considering the timespan, but we could not tell what is clearly expected from a team consisting of four members.

---

### Resources

- [XR Simulation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.0/manual/xr-simulation/simulation.html)
- [FLATICON](https://www.flaticon.com/free-icons/font)
- [ISBN documentation](https://www.isbn.org/faqs_general_questions)

---

_Written by: Jakub_

---
