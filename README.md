An assignment for COMP 476 at Concordia University

The goal was to learn to program AI functionality in the Unity Engine, applying concepts such as pathfinding and decision tree behavior



PHASE 1 - Simple Seek Behavior



https://github.com/user-attachments/assets/4770e20c-f693-484b-ac9c-8c5de616d0f7



The Hero Object (Green) seeks out the Prisoner Object (Blue). Once reached, the Hero Object seeks the Goal (Red)


PHASE 2 - Custom Pathfinding



https://github.com/user-attachments/assets/4179c51b-15e7-4c98-abf3-e9dcc63f3512



While Unity has its own built-in method for implementing pathfinding, I created my own custom method for finding an obstacle-free path to a specific point using recursive raycasting.



<img width="972" height="639" alt="step2pathfinding" src="https://github.com/user-attachments/assets/6a1ff766-161d-45c0-ae00-5f66c82575de" />


This is the first frame where the Hero Object (Green) determines the best path to take in order to navigate obstacles to its target (Blue).

Beginning at the Hero Object, a series of DIRECT rays (red) are shot toward the target point in space. If these DIRECT rays are interrupted by any obstacle, a recursive call is made to seek out a different path.

A series of relatively short rays are shot toward the LEFT (green) of the interrupted point, until the path does not detect any obstacle. The pathfinding script then attempts a DIRECT ray until it is interrupted, and repeats the process of shooting LEFT rays.

This cycle continues until it is able to find an uninterrupted path to the target point. After which, the script attempts this same procedure, except favoring the RIGHT (blue) rays. It determines if the RIGHT path is shorter than the LEFT, and takes whichever is faster.

Finally, once an uninterrupted path is found, the apex points of each alternating ray (to corner points where BLUE/GREEN rays become RED) are used as checkpoints in the final path, and the Hero Object seeks out these checkpoints until it reaches the target.


PHASE 3 - Dynamic Walls and Guardians



https://github.com/user-attachments/assets/17293d2e-3921-4af5-8b64-e66ffc96b1b6



The Hero Object (Green) is faced with moving walls which occasionally obstruct its view. On the occasion that an object appears in the middle of its path, the Hero Object will course correct and find a new unobstructed path to the target.

A Guardian (White) npc roams around the target (Blue), and will seek out the Hero Object to destroy it. The Hero Object will slowdown if it detects a Guardian in its field of vision, slowly approaching to become detected. When the Guardian sees the Hero, it will chase it, allowing the Hero to lead it back to the Goal (Red), where the Guardian can be destroyed. This will allow the Hero Object to return to its attempt to reach the target (Blue) safely
