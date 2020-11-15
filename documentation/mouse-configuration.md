# Mouse Configuration

## Mouse movement to stick movement
When converting mouse input to stick movement it will NEVER be as smooth as if the game or device took mouse input natively. As the DS4 sticks operate on 2 axis (X and Y), and each axis has one of 256 unique values. Taking the X axis (left and right), all the way left is -127, center (idle position of a stick) is 0, and all the way right is 127. Because we are converting mouse input, we take a mouse movement and have to map it to one of the 256 values per axis.

So mouse configurations will likely specific to a combination of 
* person 
* the mouse they use
* the mouses DPI setting
* the computers mouse sensitivity setting (set via control panel or equiavlent)

## What do the different mouse settings do ?


### Enable mouse input
This will enable mouse input, AND hide the mouse cursor when the "PS remote play" application is focuses

### Mouse polling rate 
How many times a second do you want the application to measure mouse input and feed it to the Playstation.
  
Be aware that increasing this number will make it more responsive, but may mean that your sensitivity needs to be increased; this is because we do not have mouse smoothing (as in gaming people generally appear to prefar Raw Input over Mouse Smoothed input).

### Mouse sensitivity
Mouse sensitivity is the speed of a mouse pointer (or in games, your target reticle) and how fast it moves on the screen (or in games, how fast you turn and look). With increased sensitivity (ie the bigger the number), the mouse moves faster and requires less effort to get across the screen.

### Mouse X axis sensitivity
Left and right

### Mouse Y axis sensitivity
Up and Down


### Mouse distance range


### Analog stick range


### Horizontal / Vertical ratio
