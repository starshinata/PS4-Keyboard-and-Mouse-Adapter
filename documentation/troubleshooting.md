# Troubleshooing


## Recommended user environment

* Make sure that "PS Remote Play" is closed before starting the adapter <br>
  (PS4KeyboardAndMouseAdapter will start "PS Remote Play" once we detect where is it installed)

* If you have a DS4 or DS5 controller/gamepad, unplug it from computer <br>
  Either being plugged in can cause interference with this application
  
* PS4 is connected to your router via Ethernet cable <br>
  (this reduces lag, and make your PS4 more consistently find-able from the "PS Remote Play" application)

* Use video from the PS4 <br>
  Plug a HDMI cable into your PS4, and connect the other end into your monitor/TV even when using remote play <br>
  (If you choose to use the video from the "PS Remote Play" then you are adding delay/lag which is generally undesired)

* The PS4 you are trying to connect to has Remote play enabled <br>
  PS4 ->  select (Settings) -> [Remote Play Connection Settings], and then select the checkbox for [Enable Remote Play].

* The PS4 you are trying to connect to is set as your PSN account's Primary PS4 <br>
  PS4 -> select (Settings) -> [Account Management] -> [Activate as Your Primary PS4] -> [Activate].

* PS Remote Play connects via a code <br>
  see [playstation.net : Using remote play on a home network](https://manuals.playstation.net/document/gb/psvita/ps4link/viaprivate.html#:~:text=%E2%84%A2%20system%202.-,On%20your%20system%2C%20select%20(PS4%20Link)%20%3E%20%5BStart,%E2%84%A2%20system%20from%20rest%20mode)

* PS Remote Play application has its video settings set to the lowest <br>
  (This should reduce the the amount of data your PS4 has to send, and how much your Remote Play application has to receive) <br>
  Goto "PS Remote Play" application (without an active console connected) -> "Settings" via the cog symbol -> "Video Quality for Remote Play" tab -> pick either PS4 or PS5 > Resolution set to lowest




## Problems and fixes



#### (Installation) Could not write to "C:\Users\XYZ\AppData\Local\PS4KeyboardAndMouseAdapter"
Can you check two things,
* PS4KeyboardAndMouseAdapter.exe is not running
* your antivirus hasnt quantined PS4KeyboardAndMouseAdapter.exe (we have been flagged as a virus because we arent vetted by a big/known/trusted corporation). <br> We submit executable scans to https://www.virustotal.com/ for every release, and they have yet to detect a real threat.

#### (Runtime) Application says "need to install remote play" but I already have it installed
If you are on PS4KeyboardAndMouseAdapter v1.0.7 or lower, you need to upgrade to a newer version. Sometime in October 2020 Sony issued a new version of "PS Remote Play" to support PS5, that broke some things in older versions of our application.


#### (Runtime) Presentation Framework-SystemCore Error
`An unhandled exception occurred in a component in your Microsoft .NET Framework application.  If you click Continue Y, the application will ignore this error and try to continue.  Could not load the "Presentation Framework-SystemCore, Version = 4.0.0.0. Culture = neutral. PublicKey Token = b77a5c561934e089 'or its integrated code or one of its dependencies. The module was expected to contain an integrated code manifest.`

Er, wish I knew. <br>
Feel free to raise an issue at https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/issues, or message pancakeslp on our discord https://discord.gg/zH4b8p4
