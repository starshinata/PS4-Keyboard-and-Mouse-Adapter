# Release checklist


1. If you have updated any dependencies, then delete the packages folder and do a fresh build to check everything is ok.

2. Have we enabled the auto update logic in ` App.xaml.cs `

3. Write update notes

4. Have we updated the version number at 
   * ` AssemblyInfo.cs `
   * ` build.ps1 `

5. if this version number unique, and not previously released?

6. Does the installer install to the version we expect?

7. CPU analysis, are we still using only 1% of the CPU?

8. If everything committed to git ?

9. Verify things are digitally signed
   * AppData\Local\PS4KeyboardAndMouseAdapter\VERSION\PS4KeyboardAndMouseAdapter.exe 
   * starshinata\PS4-Keyboard-and-Mouse-Adapter\PS4KeyboardAndMouseAdapter\bin\Release\PS4KeyboardAndMouseAdapter.exe 
   * starshinata\PS4-Keyboard-and-Mouse-Adapter\SquirrelReleases\setup.exe
   * starshinata\PS4-Keyboard-and-Mouse-Adapter\SquirrelReleases\setup.msi

10. Do antivirus scans of via [https://www.virustotal.com/](https://www.virustotal.com/)
   * AppData\Local\PS4KeyboardAndMouseAdapter\VERSION\PS4KeyboardAndMouseAdapter.exe 
   * starshinata\PS4-Keyboard-and-Mouse-Adapter\SquirrelReleases\setup.exe
   * starshinata\PS4-Keyboard-and-Mouse-Adapter\SquirrelReleases\setup.msi

11. merge to master
