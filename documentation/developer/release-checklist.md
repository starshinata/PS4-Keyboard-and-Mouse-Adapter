# Release checklist


1. Done a clean build ?<br>
 ` ./clean.sh ; ./build.sh `

1. Does Debug still work?

1. Have we enabled the auto update logic in ` AppUpdater.cs `

1. Write update notes

1. Have we updated the version number at ` build.ps1 `

1. Is this version number unique, and not previously released?

1. Does the installer install to the version we expect?

1. CPU analysis, are we still using only 1% of the CPU?

1. Have we updated the documentation

1. Have we update the documentation screenshots (using the default profile)

1. Is everything committed to git ?

1. Verify things are digitally signed

  * AppData\Local\PS4KeyboardAndMouseAdapter\VERSION\PS4KeyboardAndMouseAdapter.exe 

  * starshinata\PS4-Keyboard-and-Mouse-Adapter\SquirrelReleases\application-setup.exe

13. Do antivirus scans of via [https://www.virustotal.com/](https://www.virustotal.com/)
   * AppData\Local\PS4KeyboardAndMouseAdapter\VERSION\PS4KeyboardAndMouseAdapter.exe 
   * starshinata\PS4-Keyboard-and-Mouse-Adapter\SquirrelReleases\application-setup.exe

14. merge to master
