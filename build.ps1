
## if this fails you may want to run AS ADMIN
## `  powershell Set-ExecutionPolicy RemoteSigned  `
## if that doesnt work try 
## `  powershell Set-ExecutionPolicy Unrestricted  `

## exit on first error
$ErrorActionPreference = "Stop"

################################
################################
## might need configuring
$CERT_DIRECTORY="D:\workspace\##certificates\github.com-pancakeslp"

$SIGN_TOOL_PATH="C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x64\signtool.exe"

## TODO get this to read from the assembly file
$VERSION="1.0.9"

################################
################################


$env:Path += ";C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\amd64\"
$env:Path += ";C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x64\"


## type is the windows equiavlent of cat
$CERT_PASSWORD=$( type ${CERT_DIRECTORY}\cert-password.txt )
$CERT_PFX="${CERT_DIRECTORY}\github.com-pancakeslp.pfx"

$GENERATED_INSTALLER_PATH="SquirrelReleases"

################################
################################

function cleanup {
  remove $GENERATED_INSTALLER_PATH
  
  remove PS4KeyboardAndMouseAdapter.*.nupkg
  
  remove PS4KeyboardAndMouseAdapter\bin\
  remove PS4KeyboardAndMouseAdapter\logs\
  remove PS4KeyboardAndMouseAdapter\obj\
  
  remove PS4RemotePlayInjection\bin\
  remove PS4RemotePlayInjection\obj\
    
}

function make-nuget-package {

  $FIND="<version>REPLACE_VERSION_REPLACE</version>"
  $REPLACE="<version>$VERSION</version>"
  $TARGET_NUSPEC_FILE="nuget\PS4KeyboardAndMouseAdapter.nuspec"
  
  remove nuget\PS4KeyboardAndMouseAdapter.nuspec

  Copy-Item nuget\PS4KeyboardAndMouseAdapter.nuspec.template.xml -Destination $TARGET_NUSPEC_FILE

  ((Get-Content -path $TARGET_NUSPEC_FILE -Raw) -replace $FIND,$REPLACE) | Set-Content -Path $TARGET_NUSPEC_FILE

  nuget\nuget.exe pack $TARGET_NUSPEC_FILE

}


function manually-sign-file {
  $FILE_NAME = $args[0]

  if(![System.IO.File]::Exists($FILE_NAME))  {
    Write-Error "file $FILE_NAME missing!" 
    exit 1
  }

  ## if something isnt going right try /debug arg
  signtool.exe sign                       `
      /a                                  `
      /f ${CERT_PFX}                      `
      /p ${CERT_PASSWORD}                 `
      /fd sha256                          `
	  /tr http://timestamp.digicert.com   `
	  /td sha256                          `
      $FILE_NAME
	
}

function remove {
  $FILE_NAME = $args[0]

  if (Test-Path $FILE_NAME) 
  {
    Remove-Item -Recurse $FILE_NAME
  }
}

function sign-executables {
  echo ""
  echo "sign-ing executables" 
  manually-sign-file  "PS4KeyboardAndMouseAdapter\bin\Release\PS4KeyboardAndMouseAdapter.exe"
  echo "signed executables"
}

function sign-installers {
echo ""
  echo "sign-ing installers" 
  
  manually-sign-file  $GENERATED_INSTALLER_PATH\setup.exe
  manually-sign-file  $GENERATED_INSTALLER_PATH\setup.msi
  echo "signed installers"
}

function squirrel {
  echo ""
  echo "squirrel-ing package ..."
  
  ## in powershell uses ` to denote continues on next line
  $COMMAND=" packages\squirrel.windows.1.9.1\tools\Squirrel.exe  --releasify \`"PS4KeyboardAndMouseAdapter.${VERSION}.nupkg\`"  --releaseDir $GENERATED_INSTALLER_PATH "
  
  powershell.exe -ExecutionPolicy Bypass -Command "$COMMAND | Write-Output"
	
  echo "squirrel-ed package!"
  ##echo "wait for popup CMD to close ... "	
  
}


################################
################################

cleanup

nuget\nuget.exe install PS4KeyboardAndMouseAdapter\packages.config -OutputDirectory packages
nuget\nuget.exe install PS4RemotePlayInjection\packages.config     -OutputDirectory packages

echo "msbuild-ing"
MSBuild.exe PS4KeyboardAndMouseAdapter.sln -p:Configuration=Release /p:VersionNumber=$VERSION
echo "msbuild done"

echo ""
Copy-Item   manualBuild\csfml-Window.dll             PS4KeyboardAndMouseAdapter\bin\Release
Copy-Item   manualBuild\default-mappings.json        PS4KeyboardAndMouseAdapter\bin\Release\mappings.json

sign-executables

echo ""

make-nuget-package

squirrel

sign-installers

