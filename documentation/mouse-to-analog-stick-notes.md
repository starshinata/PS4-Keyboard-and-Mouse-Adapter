# Mouse to analog stick notes


## Mouse movement VS analog stick movement
When converting mouse input to stick movement it will NEVER be as smooth as if the game or device took mouse input natively. As the DS4 sticks operate on 2 axis (X and Y), and each axis has one of 256 unique values. Example using the X axis, all the way left is 0, center is 127 (also the idle position of a stick), and all the way right is 256. Because we are converting mouse input, we take a mouse movement and have to map it to one of the 256 values per axis.

So mouse configurations will likely specific to a combination of 
* person 
* the mouse they use
* the mouses DPI setting
* the computers mouse sensitivity setting (set via control panel or equiavlent)

## Exploring mouse movment on PS Web browser

Firstly the web browser on PS is rubbish and very slow, but it is a useful demonstration of a mouse cursor on PS

1. set "Mouse controls Left analog stick" (in PS4KMA)
2. then unset "Mouse controls Right analog stick"(in PS4KMA)
3. then set "mouse look X axis sensitivity" and "mouse look Y axis sensitivity" to the same value (in PS4KMA)
4. and then set "Horizontal/Vertical ratio" to 1 (in PS4KMA)
5. Open then PS web browser
6. The browser's cursor should mostly feel like you are using a mouse on a PC browser
