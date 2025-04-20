# PS4 Keyboard and Mouse Adapter  Developer stuff

# Background Reading
* [How to use squirrel to make setup.exe and setup.msi installers](https://intellitect.com/deploying-app-squirrel/ )
* [How to use code signing in dotnet](https://www.twelve21.io/using-signtool-exe-to-sign-a-dotnet-core-assembly-with-a-digital-certificate/)
* [Using squirrel to code sign installers](https://github.com/Squirrel/Squirrel.Windows/blob/develop/docs/using/application-signing.md )
* [Squirrel command line documentation](https://github.com/Squirrel/Squirrel.Windows/blob/develop/docs/using/squirrel-command-line.md)
* [Easy Hook Intro](https://www.codeproject.com/Articles/27637/EasyHook-The-reinvention-of-Windows-API-hooking)


## Setup
* dot net framework 4.8
* visual studio 2022 - https://visualstudio.microsoft.com/
* Workloads ?
  * .net Development
  * Windows and web development
    (Specifically "ClickOnce Publishing Tools")
* chocolatey - https://chocolatey.org/
* libxml <br>` choco install xsltproc `


## Build

Open Command prompt

```
 powershell ./build.ps1 
```

If you get an execution policy error, try one of the following <br> in Command
prompt **AS ADMIN**, the retry the above
```
powershell Set-ExecutionPolicy RemoteSigned 
```  
```
powershell Set-ExecutionPolicy Unrestricted 
```


## Code Signing
For non self signed read https://virtualgl.org/DeveloperInfo/CodeSigningHell

If you are happy with self signed certificates
we can thank soulehshaikh9 for their pfx certificate generator https://github.com/soulehshaikh99/self-signed-certificate-generator


## Gotchas

(in order which they seem most prevalent for me)

#### BadImageFormatException when Debugging
System.TypeInitializationException: 'The type initializer for 'Pizza.KeyboardAndMouseAdapter.Backend.Config.UserSettings' threw an exception.'
BadImageFormatException: An attempt was made to load a program with an incorrect format. (Exception from HRESULT: 0x8007000B)

1. make sure your nuget packages are installed
2. Set your CPU to ANY instead of x64


#### "Could not copy \Squirrel.exe"
you need to set "SquirrelToolsPath" in your project properties

eg if you have your project as 'D:\workspace\pancakeslp\PS4-Keyboard-and-Mouse-Adapter\'

then your squirrel tools will be at  'D:\workspace\pancakeslp\PS4-Keyboard-and-Mouse-Adapter\packages\squirrel.windows.1.9.1\tools'

so then  ` SquirrelToolsPath = packages\squirrel.windows.1.9.1\tools `


#### issue where WPFSpark is not found
1. remove WPFSpark from packages.config
2. Visual Studio Code > Tools > Nuget Package Manager > Package manager Console
3. ` Install-Package WPFSpark `


#### "Markup is invalid"
1. Clean the projects
2. rebuild the solution


### Powershell ./build.ps1 cannot be loaded because running scripts is disabled on this system
run this
```
powershell Get-ExecutionPolicy -List
powershell Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
powershell Get-ExecutionPolicy -List
```

If that doesn't work read this
https://learn.microsoft.com/en-gb/powershell/module/microsoft.powershell.core/about/about_execution_policies?view=powershell-7.3

### signtool.exe not found
Error 
```
signtool.exe : The term 'signtool.exe' is not recognized as the name of a cmdlet, function, script file, or operable
program. Check the spelling of the name, or if a path was included, verify that the path is correct and try again.
At C:\workspace\PS4-Keyboard-and-Mouse-Adapter\master.move-to-dotnet-6\build.ps1:384 char:5
+     signtool.exe sign                       `
+     ~~~~~~~~~~~~
    + CategoryInfo          : ObjectNotFound: (signtool.exe:String) [], ParentContainsErrorRecordException
    + FullyQualifiedErrorId : CommandNotFoundException
```

Check in build.ps1 to see what the path for signtool is defined as, and check if the path exists

if it doesnt exist, then use  ` gitroot\installers\windows10sdk\winsdksetup.exe `

then check the path again
