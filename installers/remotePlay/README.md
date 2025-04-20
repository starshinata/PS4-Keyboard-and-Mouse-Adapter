## Running 
* each folder will have a readme

## Extracting offline installers
1. download RemotePlayInstaller.exe
2. run it, but don't install
3. Open task manager, go to Details tab, and find the installer process (it should be a "msiexec.exe")
4. Make sure you have the "Command Line" column enabled, then you'll see a process with Command Line like this
```
"C:\Windows\system32\MSIEXEC.EXE" 
  /i "C:\Users\pancakes\AppData\Local\Temp\{410A6A8A-0DAE-4249-83B3-BB35C291E69A}\RemotePlayInstaller_8.0.0.14120_Win32.msi" 
  TRANSFORMS="C:\Users\pancakes\AppData\Local\Temp\{410A6A8A-0DAE-4249-83B3-BB35C291E69A}\1033.MST"
  SETUPEXEDIR="C:\Users\pancakes\Downloads" 
  SETUPEXENAME="RemotePlayInstaller.exe"
```
5. The path `C:\Users\pancakes\AppData\Local\Temp\{410A6A8A-0DAE-4249-83B3-BB35C291E69A}\` is where the full installer has been downloaded to
6. Copy that folder, disconnect your internet, and test the installer by using .msi file
