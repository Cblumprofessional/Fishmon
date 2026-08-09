**A Fish Plays Pokemon**

Fishmon is a C# project that uses a camera to track a fish's position in an aquarium and converts it to inputs that are sent to an emulator.

Fishmon tracks the fish and determines which region of the tank the fish is in. Each region is mapped to an input in the MGBA emulator.

##How it works

The aquarium is divided into a 3x3 grid

| UP | A | START |
|LEFT | IDLE| RIGHT|
| B | DOWN | SELECT |

The 

Fishmon continously

1. Captures frames from a webcam
2. Detects the fish using OpenCVSharp
3. Determines which region the fish is in
4. Converts the region into an controller input
5. Sends the input to the emulator

There is an input cooldown to prevent rapid inputs


##Requirements##
-.Net 8.0
-Webcam
-mGBA
-A GBA Pokemon Rom (Any GBA game could technically work. Would have to add the gba bumpers input and change the grid to support the bumpers.)
-A fish
