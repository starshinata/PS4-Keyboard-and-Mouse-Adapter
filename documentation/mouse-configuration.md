# Mouse Configuration


## Terms / Glossary
* Cross hairs - think mouse cursor
* DPI - Dots Per Inch, a higher number means your mouse should have higher sensitivity
* X Axis - left and right directions
* Y Axis - Up and down directions

#### Drift and deadzones
This is not a setting but explaining it for below settings

Drift is when you think your stick is idle or reset back to central, but the game recognises input. 

Games often require you to move the stick a certain amount, before it says that cant be drift and must be interesting input. The minimum amount is generally known as a deadzone.


## Mouse movement to stick movement
When converting mouse input to stick movement it will NEVER be as smooth as if the game or device took mouse input natively. As the DS4 sticks operate on 2 axis (X and Y), and each axis has one of 256 unique values. Example using the X axis, all the way left is 0, center is 127 (also the idle position of a stick), and all the way right is 256. Because we are converting mouse input, we take a mouse movement and have to map it to one of the 256 values per axis.

So mouse configurations will likely specific to a combination of 
* person 
* the mouse they use
* the mouses DPI setting
* the computers mouse sensitivity setting (set via control panel or equiavlent)

## What do the different mouse settings do ?


#### Enable mouse input
This will enable mouse input, AND hide the mouse cursor when the "PS remote play" application is focuses


#### Mouse polling rate 
How many times a second do you want the application to measure mouse input and feed it to the Playstation.
  
Be aware that increasing this number will make it more responsive, but may mean that your sensitivity needs to be increased; this is because we do not have mouse smoothing (as in gaming people generally appear to prefar Raw Input over Mouse Smoothed input).


#### Mouse sensitivity
Mouse sensitivity is the speed of a mouse cursor moves on the screen (or in games, how fast you turn and look). With increased sensitivity (ie the bigger the number), the mouse moves faster and requires less effort to get across the screen.
This is comparable to increasing your mouse's DPI setting.

#### Mouse X axis sensitivity
Left and right

#### Mouse Y axis sensitivity
Up and Down

#### Mouse distance range
disclaimer: I recommend just using mouse sensitivity

This is the minimum and maximum amount of mouse input our application will accept per mouse poll (which happens many times a second).


#### Analog stick range
disclaimer: I recommend just using mouse sensitivity

Think of this as deadzone management for your emulated stick.

Analog stick range is to allow you to set the minimum (and maximum) percentage of input stick takes from our mouse input conversion.

If you want lots of input you want a range of say 15%-95%

If you want to be very precise you might find you want 30%-40%


#### Horizontal / Vertical ratio
disclaimer: I recommend just using mouse sensitivity

This is for scaling down the Horizontal input (X axis).

Unless you have a config that works that uses this setting, we recommend using the value of 1 (no scaling)

This one is left in for legacy reasons, incase you found that you had a config that works and didnt want to fiddle it.
