# Mouse Configuration


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


#### Mouse Look X axis sensitivity
Left and right


#### Mouse Look Y axis sensitivity
Up and Down


#### Aim Toggle
In an FPS games on a console, you would normally hold L2 to 'Aim' or 'Aim Down Sight'
Some games allow you to have this as a toggle, for when that is not available we have this option.


#### Aim Toggle Retoggle Delay 
The time milliseconds before we toggle aim toggle
Set this too low, and keep holding the button for L2, and you will appear as if you cant decide between aiming or not aimining.


#### Enable 'Aim' specific sensitivity
Would you like to have a seperate sensitivity for when you Aim?


#### Mouse Aim X axis sensitivity
Left and right (When Aiming and Aim specific sensitivity is enabled)


#### Mouse Aim Y axis sensitivity
Up and Down (When Aiming and Aim specific sensitivity is enabled)


#### Mouse Wheel Scroll Hold Duration
A scroll on a mouse wheel is instantaneous. However it should be considered as pressing a controller button for X time. This slider is to set how long the button should be held for.


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
