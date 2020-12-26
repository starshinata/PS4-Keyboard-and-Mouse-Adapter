
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

## TODO get this to read from the assembly file
$VERSION="1.0.12"

################################
################################

## Path for MSBuild.exe
$env:Path += ";C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\amd64\"

## Path for MSTest.exe
##$env:Path += ";C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE"

## Path for signtool.exe
$env:Path += ";C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x64\"

## Path for vstest.console.exe
$env:Path += ";C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow"


## type is the windows equivalent of cat
$CERT_PASSWORD=$( type ${CERT_DIRECTORY}\cert-password.txt )
$CERT_PFX="${CERT_DIRECTORY}\github.com-pancakeslp.pfx"

$GENERATED_INSTALLER_PATH="SquirrelReleases"

################################
################################

function build-msbuild {

  echo "msbuild-ing"
      
  MSBuild.exe PS4KeyboardAndMouseAdapter.sln `
    -p:Configuration=Release                 `
    -p:VersionNumber=$VERSION

  if ( $LASTEXITCODE -ne 0) {
    echo "msbuild failed"
    exit $LASTEXITCODE 
  }

  echo "msbuild done"
}


function cleanup {
  remove $GENERATED_INSTALLER_PATH
  
  remove PS4KeyboardAndMouseAdapter.*.nupkg
  
  remove PS4KeyboardAndMouseAdapter\bin\
  remove PS4KeyboardAndMouseAdapter\logs\
  remove PS4KeyboardAndMouseAdapter\obj\
  
  remove PS4RemotePlayInjection\bin\
  remove PS4RemotePlayInjection\obj\

  remove TestResults
}

function dependencies-nuget {
  nuget install PS4KeyboardAndMouseAdapter\packages.config -OutputDirectory packages
  error-on-bad-return-code	

  nuget install PS4RemotePlayInjection\packages.config     -OutputDirectory packages
  error-on-bad-return-code	

  nuget install UnitTests\packages.config -OutputDirectory packages
  error-on-bad-return-code	
}


function error-on-bad-return-code {
  if ( $LASTEXITCODE -ne 0) {
    echo "error-on-bad-return-code!"
    exit $LASTEXITCODE 
  }
}


function make-nuget-package {

  $FIND="<version>REPLACE_VERSION_REPLACE</version>"
  $REPLACE="<version>$VERSION</version>"
  $SOURCE_NUSPEC_FILE="manualBuild\nuget\PS4KeyboardAndMouseAdapter.nuspec.template.xml"
  $TARGET_NUSPEC_FILE="manualBuild\nuget\PS4KeyboardAndMouseAdapter.nuspec"
  
  remove $TARGET_NUSPEC_FILE

  Copy-Item $SOURCE_NUSPEC_FILE -Destination $TARGET_NUSPEC_FILE

  ((Get-Content -path $TARGET_NUSPEC_FILE -Raw) -replace $FIND,$REPLACE) | Set-Content -Path $TARGET_NUSPEC_FILE

  nuget pack $TARGET_NUSPEC_FILE
  error-on-bad-return-code

}


function manually-sign-file {
  $FILE_NAME = $args[0]

  if(![System.IO.File]::Exists($FILE_NAME)) {
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

  error-on-bad-return-code	
}

function nuget {
  manualBuild\nuget\nuget.exe $args
}

function remove {
  $FILE_NAME = $args[0]

  if (Test-Path $FILE_NAME) {
    Remove-Item -Recurse $FILE_NAME
  }
}


function sign-executables {
  echo ""
  echo "sign-ing executables" 
  manually-sign-file  "PS4KeyboardAndMouseAdapter\bin\Release\PS4KeyboardAndMouseAdapter.exe"
  echo "signed executables"
}


function sign-installer {
  echo ""
  echo "sign-ing installer" 
  
  manually-sign-file  $GENERATED_INSTALLER_PATH\setup.exe

  echo "signed installer"
}


function squirrel {
  echo ""
  echo "squirrel-ing package ..."
  
  ## in powershell uses ` to denote continues on next line
  $COMMAND=" packages\squirrel.windows.1.9.1\tools\Squirrel.exe  --releasify \`"PS4KeyboardAndMouseAdapter.${VERSION}.nupkg\`"  --releaseDir $GENERATED_INSTALLER_PATH "
  
  powershell.exe -ExecutionPolicy Bypass -Command "$COMMAND | Write-Output"
  error-on-bad-return-code	

  ## squirrel makes an MSI, but the MSI seems to do nothing
  remove $GENERATED_INSTALLER_PATH\setup.msi

  echo "squirrel-ed package!"
}


function test-vstest {

  echo "vstest-ing"
  $UNIT_TEST_DLL="UnitTests\bin\Release\UnitTests.dll"

  if (!(Test-Path $UNIT_TEST_DLL )) {
    echo "UnitTests.dll missing! ... path $UNIT_TEST_DLL"
    exit 1
  }

  vstest.console.exe $UNIT_TEST_DLL --ListTests
  echo ""

  vstest.console.exe $UNIT_TEST_DLL UnitTests\bin\Release\csfml-Window.dll
  
  if ( $LASTEXITCODE -ne 0) {
    echo "vstest failed"
    exit $LASTEXITCODE 
  }

  echo "vstest done"
}


function valid-xaml-xmllint {
  echo ""
  echo "validating xamls xmllint"

  $files =  Get-ChildItem -recurse *.xaml | where {! $_.PSIsContainer}

  foreach ($file in $files) {
 
    manualBuild\libxml\bin\xmllint.exe $file.FullName  --noout

    if ( $LASTEXITCODE -ne 0) {
      echo "ERROR for file $file"
      exit $LASTEXITCODE 
    }

  }

  echo "validated xamls xmllint"
}


################################
################################


cleanup

dependencies-nuget

valid-xaml-xmllint

build-msbuild

test-vstest

echo ""
Copy-Item                   profiles\default-profile.json            PS4KeyboardAndMouseAdapter\bin\Release\profile-previous.json
Copy-Item  -recurse -Force  profiles                                 PS4KeyboardAndMouseAdapter\bin\Release\profiles              

sign-executables

echo ""

make-nuget-package

squirrel

sign-installer

