**A Fish Plays Pokemon**

Fishmon is a C# project that uses a camera to track a fish's position in an aquarium and converts it to inputs that are sent to an emulator.

Fishmon tracks the fish and determines which region of the tank the fish is in. Each region is mapped to an input in the MGBA emulator.

## How it works

The aquarium is divided into a 3x3 grid
| | | | 
| --- | --- | --- |
| UP | A | START |
|LEFT | IDLE| RIGHT|
| B | DOWN | SELECT |

The layout can be changed easily

Fishmon continously

1. Captures frames from a webcam
2. Detects the fish using OpenCVSharp
3. Determines which region the fish is in
4. Converts the region into an controller input
5. Sends the input to the emulator

There is an input cooldown to prevent rapid inputs


## Requirements ##
- .Net 8.0
- Webcam
- mGBA
- A GBA Pokemon Rom 
- A fish

Any GBA game could technically work. Games that require the bumpers would require adding those inputs and changing the grid.
