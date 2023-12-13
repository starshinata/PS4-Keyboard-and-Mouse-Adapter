# Troubleshooting


## Recommendations

* Don't use a HP Omen laptop<br>
  HP use a fork of [ViGEm](https://github.com/nefarius/ViGEmBus) with conflicts with the ViGEm we require<br>
  For more details see https://github.com/nefarius/ViGEmBus/issues/99

* Don't Windows Server as your OS<br>
  Windows Server has alot of issues with controller/gamepad support <br>
  For more details see https://github.com/nefarius/ViGEmBus/issues/153

* If you have Comodo Firewall, please disable HIPS <br>
  If you leave HIPS enabled, launching PS4KMA may crash your computer <br>
  https://help.comodo.com/topic-72-1-451-4760-.html <br>
  https://www.comodo.com/home/internet-security/free-internet-security.php
  
* If you have Input mapper, please uninstall it <br>
  Input mapper being installed can cause interference with this application  <br>
  https://beta.inputmapper.com/  

* If you have Rewasd installed, please uninstall it <br>
  Rewasd being installed can cause interference with this application  <br>
  https://www.rewasd.com/
    
* Make sure that "PS Remote Play" is closed before starting the adapter

* Do not start "PS Remote Play" yourself <br>
  If you start "PS Remote Play" yourself, the adapter be able to connect to that instance of remote play <br>
  (PS4KeyboardAndMouseAdapter will start "PS Remote Play" once we detect where it is installed)  

* If you have a DS4 or DS5 controller/gamepad, unplug it from computer <br>
  Either being plugged in can cause interference with this application
  
* PS is connected to your router via Ethernet cable <br>
  (this reduces lag, and make your PS more consistently find-able from the "PS Remote Play" application)

* Use video from the PS <br>
  Plug a HDMI cable into your PS, and connect the other end into your monitor/TV even when using remote play <br>
  (If you choose to use the video from the "PS Remote Play" then you are adding delay/lag which is generally undesired)

* The PS4 you are trying to connect to has Remote play enabled <br>
  PS4 ->  select (Settings) -> [Remote Play Connection Settings], and then select the checkbox for [Enable Remote Play].

* The PS4 you are trying to connect to is set as your PSN account's Primary PS4 <br>
  PS4 -> select (Settings) -> [Account Management] -> [Activate as Your Primary PS4] -> [Activate].

* PS Remote Play connects via a code (LAN mode) <br>
  see [how do I connect to Remote Play on LAN](how%20do%20I%20connect%20to%20Remote%20Play%20on%20LAN.md)

* PS Remote Play application has its video settings set to the lowest <br>
  (This should reduce the the amount of data your PS4 has to send, and how much your Remote Play application has to receive) <br>
  Goto "PS Remote Play" application (without an active console connected) -> "Settings" via the cog symbol -> "Video Quality for Remote Play" tab -> pick either PS4 or PS5 > Resolution set to lowest


## Problems and fixes
#### (Installation) "Installation has failed - Failed to extract installer"
<img src="squirrel-installer-failed-to-extract.png" alt="Failed to extract installer" width="280px"/>
Sounds like your Anti virus doesnt like the installer. <br>
Either disable your anti virus for the duration of the install, or look for a "application-extract-me.zip" in the latest release, for example  https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/releases/download/2.2.0/application-extract-me.zip


#### (Installation) "Installation has failed - something with log"
<img src="squirrel-installer-failed.png" alt="installer failed" width="280px"/>

Click on "Open Setup Log" and look for something meaningful, if you cant find something, then share the log via a github issue (https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/issues), or on our discord (https://discord.gg/zH4b8p4)


#### (Installation Log) "Could not write to C:\Users\\{USER}\AppData\Local\PS4KeyboardAndMouseAdapter" or "Couldn't write out staging user ID, this user probably shouldn't get beta anything"
Can you check three things,
* PS4KeyboardAndMouseAdapter.exe is not running
* PS Remote play is not running
* your antivirus hasnt quarantined PS4KeyboardAndMouseAdapter.exe (we have been flagged as a virus because we arent vetted by a big/known/trusted corporation). <br> We submit executable scans to https://www.virustotal.com/ for every release, and they have yet to detect a real threat.

If that doesnt resolve it, please delete the folder "C:\Users\\{USER}\AppData\Local\PS4KeyboardAndMouseAdapter", restart your PC, then try again


#### (Runtime) Presentation Framework-SystemCore Error
`An unhandled exception occurred in a component in your Microsoft .NET Framework application.  If you click Continue Y, the application will ignore this error and try to continue.  Could not load the "Presentation Framework-SystemCore, Version = 4.0.0.0. Culture = neutral. PublicKey Token = b77a5c561934e089 'or its integrated code or one of its dependencies. The module was expected to contain an integrated code manifest.`

Er, wish I knew. <br>
Feel free to raise an issue at https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/issues, or message pancakeslp on our discord https://discord.gg/zH4b8p4
