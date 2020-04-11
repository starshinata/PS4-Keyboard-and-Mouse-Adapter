# PS4 Keyboard and Mouse Adapter 

## :rocket: Click --> [here](https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/releases/download/1.0.3/Setup.exe) <-- to Download!

I didn't expect a single person to find out about this repo but you guys somehow did, so I added a UI, made the configuration process more user-friendly, and also created a setup for those of you that don't know how to build the code and just want to play.

If you think something doesn't work or is not good enough, don't get too mad at me and don't hesitate to tell me about it in our Discord server https://discord.gg/zH4b8p4 or create an issue [here](https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/issues/new/choose)

## How to use

Just download and run it. It will automatically do all the configuration stuff for you (if you get into trouble and need help, join the discord server I linked above).

## To-do list for the next release
- Make mouse settings configurable
- Improve the UI
- Fix a nasty injection crash that appears when dealing with RemotePlay that has been patched by another software 

## Why this and not REM4P?

I learned about REM4P's existence after I wrote this tool. I checked it out and I have to give props to the dev - he's done a damn good job. The app works good, great website, documentation, and tutorial. Things you might not like are: it requires a plugged-in DS4 controller at all times instead of emulating it, also it's paid, no trial, non-refundable, and the forums are locked for reading if you are not a member.

What this app does that REM4P doesn't:
- fully emulates a ds4 controller - you don't need to buy one and keep it plugged-in
- it's free

## Credits

- [PS4Macro](https://github.com/komefai/PS4Macro) - Big thanks to komefai for making and open-sourcing this tool. I've grown a lot as a developer by reading and learning from his code. PS4Macro served as a basis for this repo. I'd never have done this if hadn't stumbled on it. Komefai is MIA for 2 years and his repo is not supported anymore but you can still write pretty good bots with it, definitely check it out if you are into that kind of stuff
- [EasyHook](https://easyhook.github.io) - The best tool for Windows API hooking. Works flawlessly - from the assembly injection, to the hook trampoline code. ~~I haven't had a single problem with it~~ I had one but that doesn't make EasyHook any less cool
- [Jays2Kings/DS4Windows](https://github.com/Jays2Kings/DS4Windows) - don't need to explain that one
