
# TLDR KMA V3 Process Injection

When KMA starts, `Injector.Inject()` will inject a DLL into PsRemotePlay using EasyHook's Injection API

Once injected the DLL looks for something matching the Hook interface, which we store in `Hooks.cs`
Then some method patching will done on the RemotePlay process's methods via `PatcherGamepad.cs`

Now we wait for the RemotePlay to connect to a PlayStation and ask for button inputs.
Once ready for button inputs that will trigger `PatcherGamepad.ReadFile_Hook()` which will trigger `InjectionInterface.OnReadFile` which will trigger `GamepadProcessor.OnReceiveData()` (via InterceptionDelegate in `Injector.cs`)

`GamepadProcessor.OnReceiveData()` calls `GamepadProcessor.GetState()` which then polls the keyboard and mouse for what buttons are pressed


code\PS4KeyboardAndMouseAdapter\Backend\IpcServer\RemotePlayInjector.cs
code\PS4RemotePlayInjection\Hooks.cs
code\PS4RemotePlayInjection\InjectionInterface.cs
code\PS4RemotePlayInjection\Injector.cs
code\PS4RemotePlayInjection\PatcherGamepad.cs



# TLDR KMA V4 Process Injection

When KMA starts, `Injector.Inject()` will inject a DLL into PsRemotePlay using EasyHook's Injection API

Once injected the DLL looks for something matching the Hook interface, which we store in `Hooks.cs`
Then some method patching will done on the RemotePlay process's methods via `PatcherGamepad.cs`

Now we wait for the RemotePlay to connect to a PlayStation and ask for button inputs.
Once ready for button inputs that will trigger `PatcherGamepad.ReadFile_Hook()` which will trigger `InjectionInterface.OnReadFile` which will trigger 
//TODO is this still right in V4?
`GamepadProcessor.OnReceiveData()` (via InterceptionDelegate in `Injector.cs`)

`GamepadProcessor.OnReceiveData()` calls `GamepadProcessor.GetState()` which then polls the keyboard and mouse for what buttons are pressed


code\PS4KeyboardAndMouseAdapter\Backend\IpcServer\RemotePlayInjector.cs
code\PS4RemotePlayInjection\Hooks.cs
code\PS4RemotePlayInjection\InjectionInterface.cs
code\PS4RemotePlayInjection\Injector.cs
code\PS4RemotePlayInjection\PatcherGamepad.cs


# Where can I find code for  ... 

## PROJECT PS4KeyboardAndMouseAdapter

### capturing keyboard and mouse inputs
see code\PS4KeyboardAndMouseAdapter\Backend\GamepadProcessor.cs

### sending instructions to "RemotePlay"
see code\PS4KeyboardAndMouseAdapter\Backend\IpcServer\SendToRemotePlayIpcServiceImpl.cs


## PROJECT PS4RemotePlayInjection

