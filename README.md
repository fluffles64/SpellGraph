# SpellGraph

## Overview
**SpellGraph** is a node-based RPG ability creation tool made in Unity using the GraphView API. It allows developers to easily create abilities without the need for scripting and can be repurposed to suit various project needs.
<br><br>SpellGraph comes with an additional tool, **RealmsTool**, which allows to visualize and manage custom data types in Unity. Both tools can be used separately and you can opt not to install RealmsTool when importing the .unitypackage.
<br><br>It was developed using the Unity GraphView API, which may be subject to changes in the future. The current release was developed and tested for Unity version 2022.3.7f1.

## Features
- **User-friendly node-based Interface**: Create complex abilities without writing a single line of code.
- **Flexible and Extensible**: Designed to be easily repurposed for different projects.
- **Sample Nodes**: A total of 26 sample nodes to get you started.
- **Key Features**:
  - Effect serialization as ScriptableObject
  - Integration with RealmsTool
  - Color coded nodes and ports
  - Styling very similar to Unity’s default environment
  - Resizable window that fits all resolutions
  - Certain ports can only be connected to specific data types
  - Node search window using reflection to find all nodes and categories
  - Strong separation of editor and runtime code
  - No runtime reflection
  - Optimized runtime algorithm to run through nodes with little boilerplate code
  - Blackboard with support for up to 8 different instance variable data types
  - Real time renaming and value changing of instance variables
  - Instance variables drag/drop
  - Runtime-changing instance variables
  - Copy/paste support for nodes, instance variables and groups via JSON
  - Accidental save protection
  - Minimap
  - Effect picker
  - Groups

## Node documentation

| **Name** | **Type** | **Description** |
|----------|----------|-----------------|
| **Special** |
| `ExtendedNode` | Node, INodeData | Base class all nodes must extend from. |
| `RootNode` | ExtendedNode | Sends an output pulse whenever the effect triggers. Always present by default and cannot be moved or deleted. |
| `NodeGroup` | Group | Organizes nodes inside. Its title can be edited. |
| `VariableNode` | ExtendedNode | Special pill-like nodes representing instance variables. They can be dragged and dropped from the blackboard. |
| **General** |
| `DebugNode` | ActionNode | Takes a generic value as input and debugs its value in the console when it triggers. |
| `SetVarNode` | ActionNode | Dynamically selects a variable from the blackboard and assigns it a generic value determined by the input. |
| **Conditional** |
| `BoolNode` | ConditionNode | Splits the flow of execution depending on the value of an input boolean. |
| `ComparisonNode` | ActionNode | Compares two float/int values (`<`, `>`, `<=`, `>=`, `==`, `!=`) and returns a boolean. |
| **Math/Basic** |
| `AddNode` | ActionNode | Returns the sum of two input values. |
| `SubtractNode` | ActionNode | Returns the result of input A minus input B. |
| `MultiplyNode` | ActionNode | Returns the result of input A multiplied by input B. |
| `DivideNode` | ActionNode | Returns the result of input A divided by input B. |
| `PowerNode` | ActionNode | Returns the result of input A to the power of input B. |
| `SquareRootNode` | ActionNode | Returns the square root of input A. |
| **Events** |
| `OnAutoAttackNode` | EventNode | Sends out a pulse whenever the player auto attacks. |
| **Time** |
| `BuffNode` | ActionNode | Adds a buff or debuff icon to the top right side of the player’s screen, showing its remaining duration and a tooltip. |
| `WaitNode` | StateNode | Waits for x seconds. |
| **Combat** |
| `DamageNode` | ActionNode | Deals X physical/magic/true damage to the player. Returns the resulting value. |
| `DOTNode` | ActionNode | Deals X physical/magic/true damage over time in the form of ticks, over a duration of Y seconds, at a rate of Z seconds. It can stack over similar effects. |
| `ResetAutoAttackNode` | ActionNode | Resets the auto attack timer of the player. |
| `TeleportNode` | ActionNode | Teleports the player to a given position or to a position relative to its target. |
| **Stats** |
| `GetStatNode` | ActionNode | Returns the current value of any given stat in either the player or its target. |
| `SetStatNode` | ActionNode | Sets the current value of any given stat in either the player or its target. |
| **SFX** |
| `SFXNode` | ActionNode | Plays a random audio clip from a given list. For each audio clip, its pitch and volume can be changed. |
| **VFX** |
| `VFXNode` | ActionNode | Spawns a GameObject prefab on a child transform of a prefab for a set amount of time. For it to work, the specified bone must have the “VFXTransform” tag. |
| `AnimationNode` | ActionNode | Plays one shot of an animation given an animation controller. Its duration can be specified. |

## Installation
1. **Clone the repository**:
    ```bash
    git clone https://github.com/fluffles64/SpellGraph.git
    ```
2. **Import the Package directly into Unity**:
    - Download the latest `.unitypackage` from the [Releases](https://github.com/fluffles64/SpellGraph/releases) page.
    - Open your Unity project.
    - Go to `Assets > Import Package > Custom Package...`.
    - Select the downloaded `.unitypackage` file.

## Usage
1. **Open the SpellGraph Editor**:
    - Navigate to `Tools > SpellGraph` in the Unity menu.
2. **Create Abilities**:
    - Use the node-based interface to create and connect nodes.
    - Press space to open the node library search window.
    - Move around with the middle mouse button.
    - Copy paste with Ctrl+C/Ctrl+V
    - Drag and drop instance variables into the editor window.
    - Double click an instance variable in the Blackboard to rename it.
    - Save and test your abilities within your game.

## Contributing
1. **Fork the repository**.
2. **Create a new branch**:
    ```bash
    git checkout -b feature/YourFeature
    ```
3. **Commit your changes**:
    ```bash
    git commit -m 'Add some feature'
    ```
4. **Push to the branch**:
    ```bash
    git push origin feature/YourFeature
    ```
5. **Open a pull request**.

## License

This project is licensed under the GNU General Public License v3.0.

## Third-Party Code

This project includes third-party code. Please see the respective files for individual licenses and attributions.
