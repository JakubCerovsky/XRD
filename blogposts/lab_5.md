# Lab Day 5

## Introduction

In the second week of out development process, we have continued to work on our parts individually aiming to combine them together during the last lab.

---

## Progress

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

## Reflections

---

## Resources

- [VR Development](https://learn.unity.com/pathway/vr-development?version=6.2)

---

_Written by: NAME_

---
