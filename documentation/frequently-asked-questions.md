# Frequently Asked Questions

### I need help
See our troubleshooting instructions [troubleshooting.md](troubleshooting.md)


### Mouse moves like a stick
aka
* How do I fix the reverse acceleration 
* How do I increase mouse sensitivity so I can quickly turn 180 degrees?
* When I move the mouse slowly it goes faster, and when I move it fast it goes slow
* Why can't I do a quick 180 turn


I cant fix this, this is a limitation of console controllers.

For a better explanation watch the first minute of https://youtu.be/KtBHArN3QW0 


---

Now questions ordered alphabetically


### Can I get a profile for *INSERT GAME*

We have a short list of profiles at 
https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/blob/master/profiles/

"I am looking for the perfect settings for ..." <br>
Imagine you have walked in to a  shoe shop, and you ask for the perfect shoe. <br>
The shoe salesman will likely tell you the perfect shoe for you will be specific to you. <br>
If you are curious how settings can depend on the user please have a read of [mouse-configuration.md](mouse-configuration.md)


### Can I get banned from PSN for using this application? 
I cannot find a specific text saying this is a bannable offense. But strictly because it is an unauthorised third party application I would not be surprised if Sony claimed that using this is a banable offense.

**No one has reported being banned for it.**


### Can this invalidate my Playstation warranty?

From the PS4 License agreement (checked 2020.11.24)

Source: https://www.playstation.com/en-us/legal/softwarelicense/

`
8. USE OF UNAUTHORIZED PRODUCTS. The use of software, peripherals or other products not authorized by SIE LLC may damage your PlayStation system and invalidate your PlayStation system warranty. Only official or licensed software and peripherals should be used with your PlayStation system (e.g., in the controller ports and memory card slots).
`

**Because this software is not authorised by Sony, this may technically "invalidate your PlayStation system warranty"**


### Does my PS need to be modded or jail breaked to use this?
No!


### Does this support Fortnite?

Yes, but why do you want that?!<br>

Fortnite on Consoles will natively support keyboard and mouse. <br>
For example see
* https://www.coolblue.be/en/advice/best-fortnite-accessories.html
* https://screenrant.com/fortnite-mouse-keyboard-pros-cons-gameplay-pc/
* https://playstation.fandom.com/wiki/List_of_PlayStation_4_games_with_keyboard_and_mouse_support
* https://support.xbox.com/en-US/help/hardware-network/accessories/mouse-keyboard

Native support will be better than this adapter.


### Does this work on a Laptop
Yes - Install this adapter on your computer, and then conect to your PS via Remote Play


### Does this work on Linux
No not yet


### Does this work on Mac
No not yet


### Does this work on PS3
No - since you cant (AFAIK) conect to a PS3 via Remote Play


### Does this work on PS4
Yes - Install this adapter on your computer, and then conect to your PS4 via Remote Play


### Does this work on PS5
AKA 
* Is DualSense supported?

Yes - Install this adapter on your computer, and then conect to your PS5 via Remote Play <br>
<br>
BUT note the developer doesn't have a PS5, so can't help with PS5 issues <br>
ALSO note originally Sony was not going to let you use PS4 controllers for PS5 games and vice versa, but Sony changed thier mind on this


### Does this work on XBOX
No, but it's something we are am considering


### How do I connect to my PS on my home network
see https://manuals.playstation.net/document/gb/psvita/ps4link/viaprivate.html


### How do I increase mouse sensitivity so I can quickly turn 180 degrees?
You cant

This is a limitation of console controllers. For a better explanation watch the first minute of https://youtu.be/KtBHArN3QW0 


### How do I make PS Remote Play fullscreen
see [this](how-to-make-remote-play-fullscreen.md) <br>


### How do I set the volume for individual apps on windows?
Not a PS4KMA specific question, but have you tried 
https://www.howtogeek.com/244963/how-to-adjust-the-volume-for-individual-apps-in-windows/

But also have a look at the "miscellaneous settings" tab.


### How do I uninstall PS4 KMA?
Goto `%AppData%\..\Local\PS4KeyboardAndMouseAdapter` in Windows explorer and delete everything there


### How do I uninstall ViGEm?
see <a href="vigem.md">vigem.md</a>


### How do I use macros?
We (PS4KMA) dont explicitly support macros, but we have been asked enough times... <br>
Auto Hot Key (AHK) has been tried and is usually not accommodating. <br>
Discord user Domenick027#3614 has had success using https://www.macrorecorder.com/ <br>
Alternatively try https://github.com/komefai/PS4Macro or https://github.com/evilC/AHK-ViGEm-Bus


### I have a question or issue, why haven't you replied?
We do this in our free time, and we normally have 9-5 day jobs to contend with.

I would like to remind you this application is **free**.


### If I have a modded or jailbroken PS, can I still use this?
We have been informed you can, and no extra action is needed from normal (assuming you have a PS remote play that connects to your modded PS)


### Is using a mouse and keyboard cheating?
IMO No.

Sony has licensed Hori products https://www.playstation.com/en-us/explore/accessories/specialty-controllers/, and Hori has provided mouse and keyboard options https://www.amazon.co.uk/HORI-Tactical-Assault-Commander-Pro/dp/B01K7JEII8

If you are using an application to improve you aim or response time, that could be considered cheating. The developers of this application consider this an accessibility aid for people who have issues using a controller (I personally have RSI and cannot play with a controller for long periods of time, but can use a mouse and keyboard all day).


### What are the best settings to play an FPS game
Read [mouse-to-analog-stick-notes.md](mouse-to-analog-stick-notes.md) <br>
Try https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/blob/master/profiles/pancakes-destiny-profile.json


### Will this trigger Riot Valorant's Vanguard Anti Cheat?
https://support-valorant.riotgames.com/hc/en-us/articles/360046160933-What-is-Vanguard-

Assumption: we do not expect you to be playing Valorant, while you have PS4KMA (our adapter) running.

Vanguard is running all the time. Vanguard anticheat should not be triggered by our adapter. If it is triggered by our adapter (and the above assumption holds true) then there are much bigger privacy concerns about running the Vanguard Anticheat.

If you play Valorant with PS4KMA active then it *may* detect you as trying to cheat. It could be considered on par to using Discord's voice overlay (the overlay to see who is speaking). We dont see how capturing keyboard and mouse input could be used for cheating, but Vanguard is proprietary and we cannot look at it's inner workings.

