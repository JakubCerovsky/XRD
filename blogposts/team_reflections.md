# Reflections

## Jakub Cerovsky

## Alexandro Bolfa

Before this course, I had only used Unity for regular game development in GMD and had never touched AR or VR, so both projects felt a bit like being thrown into the deep end. In the AR project, I took over the “frontend” side, so I built most of the app’s UI flow. When the user opens the VIA library navigation app, they first see a welcome panel. Cosmin made the initial popup, and from there I added the button to hide it and built the rest of the interface on top.

After the welcome screen, the user sees a search button that opens an input field for the ISBN. Behind that, I made a small BookDatabase and the logic that checks whether the book exists in our system.

Once a book is found, the “XR” part of my work starts. I imported an asset for drawing a guidance line and adapted it so it wasn’t a fixed path but a dynamic one generated using the NavMesh baked from our 3D scan of the VIA library. The line updates as the user moves, follows walkable areas, and disappears if they cancel navigation. Getting this to work made many things from the course suddenly click: tracking noise, drift, imperfect scans and occlusion, all the things that complicate what seems like a simple pathfinding feature.

The biggest lesson I learned was that XR simulation in Unity is not reality. In the editor, everything looked perfect. On the phone, things broke in ways we didn’t expect. We had to test constantly, rebuild over and over, and accept that AR only truly behaves on a real device with a real camera and real tracking.

For the VR project, my work became much more physical and hands-on. I took ownership of the electrical box puzzle. First, I edited the model in Blender, removed an unnecessary lever, patched the mesh, separated the lid from the base, and positioned four screws around the corners to make holes in the lid's mesh. That cleanup was necessary so user wouldn't thik he can interact with the lever.

In Unity, I combined a drill body with two different tips (flat and cross) to create realistic electric screwdrivers. The screws on the box match those tips, so the user actually has to pick the right tool for each screw. I set up the logic so that when the spinning drill tip touches a screw and the user presses the trigger, the screw gradually backs out. Once all screws are removed, the lid becomes free and it falls on the ground, revealing the content of the box.

Inside the box, I added two metal parts connected with four wires. Only one wire is the correct one to cut; the other three cause an “explosion.” I set up the pliers interaction so that when both jaws close on a wire, it gets marked as cut and reports back to the bomb defuser manager. This part was super satisfying because it mixed together modelling, physics, colliders, and scripting into one complete interaction.

Looking at both projects, I can clearly see how my understanding of XR changed. In AR, most of the challenge was combining digital logic with the real world, where tracking and scanning are never perfect. In VR, I had full control over the environment, but then the challenge shifted to making interactions believable: the way screws move, the way tools respond, the way cutting a wire triggers a result.

Both projects made the theory from the course feel real. I wasn’t just reading about SLAM, NavMesh, 6DOF controllers, or XR interaction patterns, I was actually building things that depended on them. I also became much more aware of hardware limits, especially in VR, where standalone headsets can’t handle heavy meshes or expensive physics.

Overall, I’ve gone from “I know a bit of Unity” to someone who can design and justify an AR flow and a VR interaction system. I’m still learning, but I now understand how to approach XR problems: think about the user’s context, know the limits of the hardware, design around them; and always test on the real device, because nothing behaves the same in the editor.
