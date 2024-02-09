Send a ray in the direction the hero is going

If the ray hits a wall, send another ray to the hero's left to see if it's possible to move in that direction. If not, send a ray to the right. If not, send a ray behind.

The hero moves in one of the 3 directions, priority given to left and right, and last resort is backtracking.

If the hero can move left (or right), make it move in that direction. As it moves, send out rays in the direction of the detected wall. 

When the ray no longer detects a wall, let the hero move toward its original target.
