# Frequently Asked Questions

Questions ordered alphabetically


### Can I get a profile for *INSERT GAME*

We have a short list of profiles at 
https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/blob/master/profiles/

"I am looking for the perfect settings for ..." <br>
Imagine you have walked in to a  shoe shop, and you ask for the perfect shoe. <br>
The shoe salesman will likely tell you the perfect shoe for you will be specific to you. <br>
If you are curious how settings can depend on the user please have a read of  https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/blob/master/documentation/mouse-configuration.md


### Can I get banned from PSN for using this application? 
I cannot find a specific text saying this is a bannable offense. But strictly because it is an unauthorised third party application I would not be surprised if Sony claimed that using this is a banable offense.

**No one has reported being banned for it.**


### Can this invalidate my Playstation warranty?

From the PS4 License agreement (checked 2020.11.24)

Source: https://www.playstation.com/en-us/legal/softwarelicense/

`
8. USE OF UNAUTHORIZED PRODUCTS. The use of software, peripherals or other products not authorized by SIE LLC may damage your PlayStation system and invalidate your PlayStation system warranty. Only official or licensed software and peripherals should be used with your PlayStation system (e.g., in the controller ports and memory card slots).
`

Because this software is not authorised by Sony, this may technically "invalidate your PlayStation system warranty"


### Does my PS4 need to be modded to use this?
No!


### How do I connect to my PS4 on my home network
see https://manuals.playstation.net/document/gb/psvita/ps4link/viaprivate.html#:~:text=%E2%84%A2%20system%202.-,On%20your%20system%2C%20select%20(PS4%20Link)%20%3E%20%5BStart,%E2%84%A2%20system%20from%20rest%20mode.


### How do I increase mouse sensitivity so I can quickly turn 180 degrees?
You cant

This is a limitation of console controllers. For a better explanation watch the first minute of https://youtu.be/KtBHArN3QW0 


### How do I make this work on PS5?
AKA 
* How do I make DualSense controller work
* Is Dualsense supported?
* Is PS5 supported?

It does (or atleast I have had several people say it works) <br>
But I do not have a PS5 console to test this myself, so I do expect some bugs to be raised. Note originally PS5 was not going to let you use PS4 controllers for PS5 games and vice versa.



### How do I set the volume for individual apps on windows?
Not a PS4KMA specific question, but have you tried 
https://www.howtogeek.com/244963/how-to-adjust-the-volume-for-individual-apps-in-windows/


### How do I uninstall PS4 KMA?
Goto `%AppData%\..\Local\PS4KeyboardAndMouseAdapter` in windows explorer and delete everything there


### How do I uninstall ViGEm?
see <a href="vigem.md">vigem.md</a>


### I have a question or issue, why havent you replied?
We do this in our free time, and we normally have 9-5 day jobs to contend with.

I would like to remind you this application is **free**.


### Is using a mouse and keyboard cheating?
IMO No.

Sony has licensed Hori products https://www.playstation.com/en-us/explore/accessories/specialty-controllers/, and Hori has provided mouse and keyboard options https://www.amazon.co.uk/HORI-Tactical-Assault-Commander-Pro/dp/B01K7JEII8

If you are using an application to improve you aim or response time, that could be considered cheating. The developers of this application consider this a accessibility aid for people who have issues using a controller (I personally have RSI and cannot play with a controller for long periods of time, but can use a mouse and keyboard all day).


### What are the best settings to play an FPS game
Read https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/blob/master/documentation/mouse-to-analog-stick-notes.md
Try https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/blob/master/profiles/pancakes-destiny-profile.json


### Will this trigger Riot Valorant's Vanguard Anti Cheat?
https://support-valorant.riotgames.com/hc/en-us/articles/360046160933-What-is-Vanguard-

Assumption: we do not expect you to be playing Valorant, while you have PS4KMA (our adapter) running.

Vanguard is running all the time. Vanguard anticheat should not be triggered by our adapter. If it is triggered by our adapter (and the above assumption holds true) then there are much bigger privacy concerns about running the Vanguard Anticheat.

If you play Valorant with PS4KMA active then it *may* detect you as trying to cheat. It could be considered on par to using Discord's voice overlay (the overlay to see who is speaking). We dont see how capturing keyboard and mouse input could be used for cheating, but Vanguard is proprietary and we cannot look at it's inner workings.
