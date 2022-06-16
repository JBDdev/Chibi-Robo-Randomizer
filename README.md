# Chibi-Robo Randomizer

Chibi-Robo Randomizer is a Windows application that randomizes the locations of equipment, quest items, trash, and more.

![Screencap of Randomizer GUI](/Randomizer/interfaceIMG.PNG)

## How to Use

- Open ISO
  - Click this button to select a NTSC-USA copy of the Chibi-Robo ISO file. (Do not ask for a download link for this, you must source it yourself!)
- Browse
  - The randomizer will create a copy of the ISO file before applying the edits to the game files. Use the Browse button to select where this file will be saved.
- Mode / Logic
  - Select a game mode / logic setting from the dropdown. Currently, only Glitchless is supported, but make sure at least something is selected in this dropdown.
- Seed
  - The seed is a series of alphanumeric characters that correspond to how the items are randomized into the game. Given the same seed, the randomizer will put the items in the same locations. This way, two people can generate the same version of a randomized game for races / co-op runs. This will be filled in automatically by default
- Free PJs
  - Checking this box will add the Pajamas as a bonus item to the Chibi-Shop for 10 Moolah, which can allow for more convenient access to areas / locations blocked by day/night-specific events.
- Open Upsatirs
  - Checking this box will insert a stack of books onto the 2nd stair in the Foyer, allowing immediate access upstairs without the need for the ladder.
- Charged Battery
  - Checking this will shuffle a fully-charged battery as a bonus item into the game (in addition to the default uncharged battery, which is needed to unlock scrap)
- Randomize
  -Click this button once finished with the above controls and the randomizer will create a new ISO at the file path you have specified with the Browse button!
  
## KNOWN ISSUES

- Giga-Charger / Battery cannot spawn upstairs if Open Upstairs is enabled
- The program will crash if any part of the file path where the program is extracted contains a space
- Randomization for the Copter, Blaster, and Radar are planned but not supported
- Duplicate items cannot be randomized to different slots in the shop (meaning frog rings cannot appear in the shop)
- If a shop location doesn't have an item randomized to it, it will not appear as an option in the shop (this is a temporary fix for the above)
