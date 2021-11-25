# PS4 Keyboard and Mouse Adapter  Developer stuff

# Background Reading
* [How to use squirrel to make setup.exe and setup.msi installers](https://intellitect.com/deploying-app-squirrel/ )
* [How to use code signing in dotnet](https://www.twelve21.io/using-signtool-exe-to-sign-a-dotnet-core-assembly-with-a-digital-certificate/)
* [Using squirrel to code sign installers](https://github.com/Squirrel/Squirrel.Windows/blob/develop/docs/using/application-signing.md )
* [Squirrel command line documentation](https://github.com/Squirrel/Squirrel.Windows/blob/develop/docs/using/squirrel-command-line.md)
* [Easy Hook Intro](https://www.codeproject.com/Articles/27637/EasyHook-The-reinvention-of-Windows-API-hooking)


## Setup
visual studio 2019

Workloads ?
* .net Development
* Windows and web develoment
  (Specifically "ClickOnce Publishing Tools")


## Build
run the powershell script

` build.ps1 `


## Code Signing
For non self signed read https://virtualgl.org/DeveloperInfo/CodeSigningHell

If you are happy with self signed certificates
we can thank soulehshaikh9 for their pfx certificate generator https://github.com/soulehshaikh99/self-signed-certificate-generator


## Gotchas

(in order which they seem most prevalent for me)

#### BadImageFormatException when Debugging
System.TypeInitializationException: 'The type initializer for 'PS4KeyboardAndMouseAdapter.Config.UserSettings' threw an exception.'
BadImageFormatException: An attempt was made to load a program with an incorrect format. (Exception from HRESULT: 0x8007000B)

1. make sure your nuget packages are installed
2. Set your CPU to ANY instead of x64


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

