# PS4 Keyboard and Mouse Adapter  Developer stuff

# Background Reading
https://intellitect.com/deploying-app-squirrel/ for how to use squirrel to make setup.exe and setup.msi installers
https://www.twelve21.io/using-signtool-exe-to-sign-a-dotnet-core-assembly-with-a-digital-certificate/ for using code signing in dotnet
https://github.com/Squirrel/Squirrel.Windows/blob/develop/docs/using/application-signing.md using squirrel to code sign installers
https://github.com/Squirrel/Squirrel.Windows/blob/develop/docs/using/squirrel-command-line.md squirrel-command-line documentation


## Setup
visual studio 2019

Workloads ?
* .net Development
* Windows and web develoment
  (Specifically "ClickOnce Publishing Tools"


## Build
run the powershell script

` build.ps1 `


## Code Signing
For non self signed read https://virtualgl.org/DeveloperInfo/CodeSigningHell

If you are happy with self signed certificates
we can thank soulehshaikh9 for their pfx certificate generator https://github.com/soulehshaikh99/self-signed-certificate-generator


## Gotchas

(in order which they seem most prevalent for me)

#### "Markup is invalid"
1. Clean the projects
2. rebuild the solution


#### "Could not copy \Squirrel.exe"
you need to set "SquirrelToolsPath" in your project properties

eg if you have your project as 'D:\workspace\pancakeslp\PS4-Keyboard-and-Mouse-Adapter\'

then your squirrel tools will be at  'D:\workspace\pancakeslp\PS4-Keyboard-and-Mouse-Adapter\packages\squirrel.windows.1.9.1\tools'

so then  ` SquirrelToolsPath = packages\squirrel.windows.1.9.1\tools `


#### issue where WPFSpark is not found
1. remove WPFSpark from packages.config
2. Visual Studio Code > Tools > Nuget Package Manager > Package manager Console
3. ` Install-Package WPFSpark `


## Release checklist

1. Have we enabled the auto update logic in ` App.xaml.cs `

2. Write update notes

3. Have we updated the version number at 
   * ` AssemblyInfo.cs `
   * ` build.ps1 `

4. if this version number unique, and not previously released?

5. Does the installer install to the version we expect?

6. If everything committed to git ?

7. Verify things are digitally signed
   * AppData\Local\PS4KeyboardAndMouseAdapter\VERSION\PS4KeyboardAndMouseAdapter.exe 
   * starshinata\PS4-Keyboard-and-Mouse-Adapter\PS4KeyboardAndMouseAdapter\bin\Release\PS4KeyboardAndMouseAdapter.exe 
   * starshinata\PS4-Keyboard-and-Mouse-Adapter\SquirrelReleases\setup.exe
   * starshinata\PS4-Keyboard-and-Mouse-Adapter\SquirrelReleases\setup.msi

8. Do antivirus scans of
   * AppData\Local\PS4KeyboardAndMouseAdapter\VERSION\PS4KeyboardAndMouseAdapter.exe 
   * starshinata\PS4-Keyboard-and-Mouse-Adapter\SquirrelReleases\setup.exe
   * starshinata\PS4-Keyboard-and-Mouse-Adapter\SquirrelReleases\setup.msi

9. merge to master
