# Version History


### v 2.1.0
* Stuff users will appreciate
  * support for adding bindings to mouse wheel scroll
  * switches/toggles for which analog stick is controlled by mouse

    * Added to "Mouse Advanced Config" tab,
    * On "Simple Config" tab, relocated the switches (bottom right with the other mouse things)
    * On "Simple Config" tab, added text, so the switches should be more obvious
  * Switches/toggles now use black/green to signify off/on (previously we used system colours, and if you had a system colour near black the toggle was a little confusing)

* Under the hood stuff
  * updated build version management (so there is only a single truth for what the current version is)
  * Fixed some bugs in the "advances mappings" screen, which hindered users from setting bindings


### v 2.0.0
<ul>

<li>
  lots of subtle changes and bug fixes <br>
  (added 5,761 lines, removed 1,210 lines)
</li>

<li>Reworked button mappings to allow
  <ul>
    <li> allow mouse buttons to be rebound</li>
    <li> allow as many mappings as the user desires</li>
  </ul>
</li>

<li>Mouse
  <ul>
  <li>Added aim specific mouse sensitivity</li>
  <li>Added aim Toggle</li>
  </ul>
</li>

<li>Remote Play settings (from inside the adapter)
  <ul>
  <li>
    Ability to reset the Remote Play window <br>
    (cause it is annoying when you accidentally drag it offscreen(</li>
  <li>
    Option to configure Remote Play volume <br>
    (this value will saved to the profile, so will be remembered on reopening adapter or loading profile)
  </li>
  </ul>
</li>

</ul>



### v 1.0.11
* You can now save and load profiles (which are a collection of bindings/settings for a particular application on PS4)

### v 1.0.10
* fixed an issue where we were unintentionally using extra CPU

### v 1.0.9
* Disclaimer: this version seems to have an increase in CPU usage, if thats a problem please use v1.0.8 instead.
* Fixed image render so controller image is smoothly scaled (controller image, and other images look less jagged/janky)
* Fixed issue where 'PS Remote Play' was still taking input, even though it wasn't focused
* Added mappings for PS Button and Touch button
* Added Keyboard mappings for right stick
* Added mouse poll rate settings
* Updated mouse anchoring logic so it is harder for you to click out of 'PS Remote Play' (still theoretically possible to click out of 'PS Remote Play', if you experience this issue try increasing the mouse poll rate, or file an issue). <br> We specifically recognise where 'PS Remote Play' window is and anchor to the centre of that - previously it was a random (not strictly random but it didn't have any obvious significance) point on your primary monitor
* Added switches to let user set which stick takes input from the mouse.<br> You could technically have move sticks controlled by the mouse ... I WOULD NOT recommend both sticks bound to the mouse!
* Added more documentation
* Lots of refactorings to make the code cleaner.

### v 1.0.8
* Updated paths for the newest version of Sony's Remote Play app

### v 1.0.7
* Mapping from mouse to analog stick is now configurable
* New UI
* Bug fixes

### v 1.0.6
* Remote Play toolbar used to pop up at the bottom of the screen every time the mouse is moved, not anymore! The toolbar hides as soon as the mouse is hidden.
* Fixed a bug that assumed English system language when looking for Remote Play installation directory

### v 1.0.5
* Added mappings for DPad up, left, down, right

### v 1.0.4
* Added configurable mappings for L2, R2, and R3
* Fixed a bug where customized mappings are not saved between sessions
* Mouse movement feels much more smooth

### v 1.0.3
* Added mappings for L1, R1, L3, R3, and Touchpad buttons
* Various bug fixes

### v 1.0.2
* Fixed a bug where the updater was leaking memory and crashing the app after some time
* Fixed a startup injection error that was happening from time to time due to concurrency issues

### v 1.0.1
* First release