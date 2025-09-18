# Unity AI Programming

An advanced AI programming assignment for COMP 476 at Concordia University, demonstrating progressive implementation of artificial intelligence concepts in the Unity Engine.

## 🎯 Project Overview

This project showcases the evolution of AI behaviors from basic seek mechanics to sophisticated multi-agent systems with custom pathfinding, dynamic obstacle avoidance, and complex decision-making algorithms. The implementation covers fundamental AI concepts including pathfinding, decision trees, field of view systems, and multi-agent coordination.

## 🚀 Key Features

### Core AI Systems
- **Custom Recursive Pathfinding Algorithm** - Self-implemented obstacle avoidance system
- **Field of View (FoV) Detection** - Cone-based vision systems for AI agents
- **Multi-Agent Coordination** - Complex interactions between Hero, Guardian, and Prisoner entities
- **Dynamic Obstacle Avoidance** - Real-time path recalculation for moving obstacles
- **State Machine Behaviors** - Flee, Hunt, and Destroy strategies

### Advanced Mechanics
- **Player Detection & Escape Systems** - AI responds to Hero presence
- **Guardian Patrol & Pursuit** - Intelligent enemy AI with wandering and chasing behaviors
- **Prisoner Rescue Mechanics** - Multi-step objective completion system
- **Dynamic Wall Navigation** - Adaptive pathfinding for moving obstacles

## 📋 Task Progression

### Task 1 - Basic Seek Behavior
**Simple AI Movement and Target Acquisition**

https://github.com/user-attachments/assets/4770e20c-f693-484b-ac9c-8c5de616d0f7

The Hero Object (Green) demonstrates fundamental seek behavior, pursuing the Prisoner Object (Blue). Upon reaching the prisoner, the hero then seeks the Goal (Red), establishing the basic AI movement foundation.

**Key Concepts:**
- Basic seek steering behavior
- Target switching logic
- Simple state transitions

### Task 6 - Advanced Pathfinding
**Custom Pathfinding**

https://github.com/user-attachments/assets/4179c51b-15e7-4c98-abf3-e9dcc63f3512

Implementation of a custom recursive pathfinding algorithm that navigates around static obstacles.

<img width="972" height="639" alt="step2pathfinding" src="https://github.com/user-attachments/assets/6a1ff766-161d-45c0-ae00-5f66c82575de" />

**Pathfinding Algorithm Details:**
- **Direct Ray Casting**: Initial attempts to reach target with straight-line paths (RED rays)
- **Recursive Obstacle Avoidance**: When blocked, system recursively explores alternative routes
- **Left/Right Path Exploration**: Systematic exploration of left (GREEN) and right (BLUE) detour options
- **Optimal Path Selection**: Algorithm compares path lengths and selects the most efficient route
- **Checkpoint Navigation**: Uses calculated waypoints for smooth movement execution


### Task 10 - Multi-Agent System with Player Interaction
**Complex AI Ecosystem with Guardian Enemy Integration**

https://github.com/user-attachments/assets/17293d2e-3921-4af5-8b64-e66ffc96b1b6

The most advanced implementation featuring dynamic walls, multiple AI agents, and player interaction systems.

**Guardian AI Features:**
- Cone-based field of view detection
- Wandering patrol behavior around protected areas
- Pursuit mechanics when Hero is detected
- Collision avoidance with other agents
- Dynamic pathfinding for obstacle navigation

**Advanced Features:**
- **Dynamic Obstacle System**: Moving walls that require real-time path recalculation
- **Player Detection & Escape**: AI agents detect and respond to human player presence
- **Multi-Strategy AI**: Hero can switch between Flee, Hunt, and Destroy behaviors
- **Guardian Luring Mechanics**: Strategic gameplay where Hero leads Guardians to destruction zones
- **Enhanced Pathfinding**: Improved algorithm with better obstacle handling and escape route calculation

## 🛠️ Technical Implementation

### Pathfinding Algorithm
The custom pathfinding system uses recursive raycasting with the following approach:
1. **Direct Path Attempt**: Cast rays directly toward target
2. **Obstacle Detection**: Identify blocking objects in the path
3. **Alternative Exploration**: Systematically explore left and right detour options
4. **Path Optimization**: Compare route lengths and select optimal path
5. **Dynamic Recalculation**: Continuously update paths for moving obstacles

### AI Behavior States
- **Seek**: Basic movement toward target
- **Flee**: Escape behavior when threatened
- **Hunt**: Aggressive pursuit of enemies
- **Avoid**: Obstacle and threat avoidance
- **Wander**: Random patrol behavior for Guardians

### Field of View System
- Cone-based vision detection using angle calculations
- Raycast-based line-of-sight verification
- Dynamic vision range and angle adjustments
- Multi-agent vision coordination

## 🎮 Gameplay Mechanics

### Hero AI
- Rescues prisoners and returns them to base
- Avoids Guardian detection using stealth mechanics
- Can lure Guardians to destruction zones
- Responds to player presence with escape behavior

### Guardian AI
- Patrols designated areas with wandering behavior
- Detects and pursues Hero when in line of sight
- Can be destroyed when led to specific zones
- Avoids collisions with other agents and obstacles

### Player Interaction
- Human player can influence AI behavior
- AI agents detect and respond to player presence
- Dynamic difficulty based on player actions

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Task 1/          # Basic seek behavior
│   ├── Task 6/          # Advanced pathfinding & Guardian AI
│   └── Task 10/         # Multi-agent system with player interaction
├── Prefab/              # Game object prefabs for each task
├── Scenes/              # Unity scenes for each task
└── Materials/           # Visual materials and textures
```

## 🎓 Learning Outcomes

This project demonstrates:
- **AI Programming Fundamentals**: Steering behaviors, state machines, and decision trees
- **Pathfinding Algorithms**: Custom implementation of obstacle avoidance systems
- **Multi-Agent Systems**: Coordination between multiple AI entities
- **Game AI Design**: Balancing challenge and player experience
- **Unity Engine Experience**: Advanced scripting and component systems

## 🚀 Getting Started

1. Open the project in Unity 2020.3.18f1 or later
2. Navigate to the desired task scene (Task 1, Task 6, or Task 10)
3. Press Play to observe the AI behaviors
4. For Task 10, use WASD keys to control the player character

## 📊 Technical Specifications

- **Engine**: Unity Editor 2020.3.18f1
- **Language**: C#
- **AI Concepts**: Pathfinding, State Machines, Field of View, Multi-Agent Systems
- **Performance**: Optimized for real-time gameplay with efficient raycasting and collision detection

---


