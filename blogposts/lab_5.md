# Lab Day 5

## Introduction

In the second week of out development process, we have continued to work on our parts individually aiming to combine them together during the last lab.

## Progress

### Task Division

**Marcus:** Timer; Keypad;

**Jakub:** Shapes puzzle; 

**Cosmin:** Added racks to the environment; Cardboard box; Blender bomb;

**Alex:** Electric box puzzle;


## Shapes Puzzle

Next thing on this part of the puzzle was to implement logic for the shape puzzle game itself: if all shapes are in correct holes -> puzzle completed.

### XR Socket Interactor

This component served as a snapping tool for the shapes. Creating an empty object with this component and placing it in the middle of each hole in the board made it easy for the shapes to snap in place. However, what about other object which are not supposed to be placed in? To solve this issue, a new layer mask was created and applied on each shape. Not this new layer mask was selected in each hole socket as Interaction Layer Mask, so the socket does not allow any other object to snap in.

Furthermore, triangle and cylinder shapes needed an additonal attachment point added, as their original rotation did not match the hole in the board. These new game object were created as children for these shapes, positioned as desired and assigned to their **XR Grab Interactable** components as Attach Transform. Socket then uses the transform information of this object to properly snap the shape in place.

### Socket Scripts

Each hole/socket needed a script which would handle the logic for the entered shape. Based on a tag in the script, if a shape matching the tag would be placed in the hole, the script would modify the hole as filled, and inform the [Shapes Puzzle Script](./#shapes_puzzle_script).

Extra logic had to be implemented, since there is a implementation behind the **XR Socket Interactor** which makes any object which snapped in the socket as kinetic, which removes the option to use gravity. So now, when any object is removed from the socket, these values are reassigned.

### Shapes Puzzle Script

This script expects an array of the Socket Scripts, and when invoked, checks through this list to verify if all the sockets are filled correctly. If everything is in place, the Shape Puzzle is marked as solved.

---

_Written by: Jakub_

---

## Cardboard Box

### Opening the box

Initially, the box was supposed to be grabbable, but it added too much complexity, so we decided to make the position static (with the excuse of it being too heavy). 
A script on the tape starts the bomb levitation and makes the cardboard box disappear when the player takes a cutter and makes a cutting motion on it. Initial functionality was similar to a button's functionality. 

---

_Written by: Cosmin_

---

## Electrical Box

The electrical box has screws, a lid, and the wires inside. Each screw keeps track of whether the drill tip is touching it and moves outward as the drill turns; when all screws are removed the lid detects that and becomes free to come off. The wires inside are monitored by a simple manager that listens for cuts — if the player snips the correct wire the puzzle is solved, otherwise the bomb triggers the failure behavior. In short: the drill and screws interact to free the lid, and the wires monitor decides whether the player cut the right wire.

### Electric screwdriver

The electric screwdriver is the player’s powered tool for removing screws. The drill objects include the visual spinning tip and the tip collider. When the spinning tip touches a screw, the tip reports rotation to the screw so the screw can advance out of its hole. A small bridge on the tip forwards collisions to the drill logic so contact is reliable. The drill also exposes an activation state (trigger pressed) so the tool only “unscrews” while the player is actively using it. Together, these parts let the player aim the drill at a screw, press the trigger, and watch the screw slowly back out.

### Pliers

The pliers are used for cutting wires. When the player closes the pliers and both sides contact a wire, the pliers perform the cut and the wire swaps to its “cut” visual. The pliers also expose simple grab and activate hooks so they work naturally with the XR interaction controls. Put simply, the pliers let the player close both jaws on a wire and cut it reliably, and that cut is reported back to the electrical box logic.

---

_Written by: Alex_

---

## Reflections

This week, we had 2 issues that slowed us down significantly: lack of unity experience and low-end hardware.
For most of the day, we focused on watching guides, understanding the template we are using and slowly using it to implement things for our parts of the project.
At this point, we realised that there were also significant differences between the way unity ran on our PCs. Running it was laggy on Marcus's laptop and building it took a really long time on Jakub's laptop (>30 min). For the next lab, testing and building will be done mostly on the other laptops.
Overall, this week was more about learning and finding ways to work with what we have.

---

_Written by: Cosmin_

---

## Resources

- [VR Development](https://learn.unity.com/pathway/vr-development?version=6.2)

