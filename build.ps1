
## if this fails you may want to run AS ADMIN
## `  powershell Set-ExecutionPolicy RemoteSigned  `
## if that doesnt work try 
## `  powershell Set-ExecutionPolicy Unrestricted  `

## param needs to be first non comment line of file
param ([string]$execGenerateArtefact='TRUE', [string]$execTest='TRUE')

echo "ARGS IN"
echo "execTest '$execTest'"
echo "execGenerateArtefact '$execGenerateArtefact'"
echo "ARGS OUT"
echo ""

################################
################################


## exit on first error
$ErrorActionPreference = "Stop"

################################
################################

## might need configuring
$CERT_DIRECTORY="D:\workspace\##certificates\github.com-pancakeslp"

#$MS_BUILD_CONFIG="Debug"
$MS_BUILD_CONFIG="Release"

$VERSION="3.2.0"

################################
################################

$VISUAL_STUDIO_PATH="C:\Program Files\Microsoft Visual Studio\2022\"

## Path for MSBuild.exe
$env:Path += ";$VISUAL_STUDIO_PATH\Community\MSBuild\Current\Bin\amd64\"

## Path for signtool.exe
$env:Path += ";C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x64\"

## Path for vstest.console.exe
$env:Path += ";$VISUAL_STUDIO_PATH\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow"

$GENERATED_INSTALLER_PATH="SquirrelReleases"

$PROJECT_DIRECTORY_COMMON="code\Common"
$PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER="code\PS4KeyboardAndMouseAdapter"
$PROJECT_DIRECTORY_PS4_REMOTE_PLAY_INJECTION="code\PS4RemotePlayInjection"
$PROJECT_DIRECTORY_TEST_TOOLS="code\TestTools"
$PROJECT_DIRECTORY_UNIT_TESTS="code\UnitTests"

$NUGET_PACKAGE_PATH="${env:HOME}\.nuget\packages\"

################################
################################

function add-build-date {
  make-dir $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\Resources\

  ## format s means sortable aka ISO 8601
  $DATETIME = (get-date -Format s)
  echo $DATETIME > $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\Resources\BuildDate.txt
}


function build-msbuild {

  echo "msbuild-ing"

  ## "-p:UseSharedCompilation=false" for CodeQL
  MSBuild.exe PS4KeyboardAndMouseAdapter.sln    `
    -p:Configuration=$MS_BUILD_CONFIG           `
    -p:FrameworkPathOverride="C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1" `
    -p:UseSharedCompilation=false               `
    -p:VersionNumber=$VERSION
    

  if ( $LASTEXITCODE -ne 0) {
    echo "msbuild failed"
    exit $LASTEXITCODE 
  }

  echo "msbuild sleep"
  ## sleep cause sometimes this step returns too early
  ## wait to make sure nothing errors
  Start-Sleep -Milliseconds 500

  echo "msbuild done"
}


function cleanup-prebuild {
  remove $GENERATED_INSTALLER_PATH
  
  remove PS4KeyboardAndMouseAdapter.*.nupkg
  
  remove $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\bin\
  remove $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\logs\
  remove $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\obj\
  
  remove $PROJECT_DIRECTORY_PS4_REMOTE_PLAY_INJECTION\bin\
  remove $PROJECT_DIRECTORY_PS4_REMOTE_PLAY_INJECTION\obj\

  remove TestResults
}


function cleanup-postbuild {
  remove PS4KeyboardAndMouseAdapter.*.nupkg
  remove TestResults
}


function dependencies-install {
  dotnet restore
  error-on-bad-return-code
}

function error-on-bad-return-code {
  if ( $LASTEXITCODE -ne 0 ) {
    echo "error-on-bad-return-code!"
    exit $LASTEXITCODE 
  }
}


function main_exec {
    dir "C:\Program Files\Reference Assemblies\Microsoft\Framework"
    dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\"
    dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework"

  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0"     
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5"     
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6"     
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.2"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7"     
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.1"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8"     
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.X"     
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0"     
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5"     
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6"     
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.2"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7"     
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.1"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8"     
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1"   
  dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.X"    
    
    add-build-date

    cleanup-prebuild

    dependencies-install

    valid-xaml-xmllint

    update-assembly-info
    build-msbuild


    echo ""
    if ( $execTest -eq "TRUE" ) {
        test-vstest
    } else {
        echo "tests SKIPPED, because arg execTest was '$execTest'"
    }
    echo ""

    if ( $execGenerateArtefact  -eq "TRUE" ) {

        echo "artefact generation STARTED"

        echo ""
        Copy-Item                   profiles\default-profile.json                                                $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\bin\$MS_BUILD_CONFIG\profile-previous.json
        Copy-Item  -recurse -Force  profiles                                                                     $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\bin\$MS_BUILD_CONFIG\profiles
        Copy-Item                   $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\application-settings.json  $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\bin\$MS_BUILD_CONFIG\application-settings.json

        sign-executables

        echo ""


        if( $MS_BUILD_CONFIG -eq "Release" ) {

          make-extract-me-installer

          make-nuget-package

          squirrel

          sign-installer
        }

        echo "artefact generation FINISHED"

    } else {
        echo "artefact generation SKIPPED, because arg execGenerateArtefact was '$execGenerateArtefact'"
    }

    cleanup-postbuild
}


function make-dir {
  $PATH = $args[0]

  if (!(Test-Path $PATH)) {
    New-Item -ItemType directory -Path $PATH
  }
}


function make-extract-me-installer {

  echo ""
  echo "making extract-me-installer"

  $EXTRACTED_PATH="$GENERATED_INSTALLER_PATH\extract-temp\"

  make-dir $EXTRACTED_PATH\app-$VERSION

  Copy-Item  -Force           $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\bin\Release\*         $EXTRACTED_PATH\app-$VERSION
  Copy-Item  -Force -Recurse  $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\bin\Release\profiles  $EXTRACTED_PATH\app-$VERSION\profiles
  Copy-Item  -Force           manualBuild\extract-me-bin\*                                            $EXTRACTED_PATH

  $compress = @{
    Path = "$EXTRACTED_PATH\*"
    DestinationPath = "$GENERATED_INSTALLER_PATH\application-extract-me.zip"
  }
  Compress-Archive @compress
  error-on-bad-return-code

  remove $EXTRACTED_PATH

  echo "made extract-me-installer"
}


function make-nuget-package {
  echo ""
  echo "making nuget-package"

  $FIND="<version>REPLACE_VERSION_REPLACE</version>"
  $REPLACE="<version>$VERSION</version>"
  $SOURCE_NUSPEC_FILE="manualBuild\nuget\PS4KeyboardAndMouseAdapter.nuspec.template.xml"
  $TARGET_NUSPEC_FILE="manualBuild\nuget\PS4KeyboardAndMouseAdapter.nuspec"

  remove $TARGET_NUSPEC_FILE

  Copy-Item $SOURCE_NUSPEC_FILE -Destination $TARGET_NUSPEC_FILE

  ((Get-Content -path $TARGET_NUSPEC_FILE -Raw) -replace $FIND,$REPLACE) | Set-Content -Path $TARGET_NUSPEC_FILE

  nuget pack $TARGET_NUSPEC_FILE
  error-on-bad-return-code
  echo "made nuget-package"
}


function manually-sign-file {
  $FILE_NAME = $args[0]

  ## type is the windows equivalent of cat
  $CERT_PASSWORD=$( type ${CERT_DIRECTORY}\cert-password.txt )
  $CERT_PFX="${CERT_DIRECTORY}\github.com-pancakeslp.pfx"

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
  manually-sign-file  "$PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\bin\$MS_BUILD_CONFIG\PS4KeyboardAndMouseAdapter.exe"
  echo "signed executables"
}


function sign-installer {
  echo ""
  echo "sign-ing installer" 
  
  manually-sign-file  $GENERATED_INSTALLER_PATH\application-setup.exe

  echo "signed installer"
}


function squirrel {
  echo ""
  echo "squirrel-ing package ..."
  

  $SQUIRREL_PATH="$NUGET_PACKAGE_PATH\squirrel.windows\1.9.1"

  $COMMAND=" ${SQUIRREL_PATH}\tools\Squirrel.exe  --releasify \`"PS4KeyboardAndMouseAdapter.${VERSION}.nupkg\`"  --releaseDir $GENERATED_INSTALLER_PATH "
  
  powershell.exe -ExecutionPolicy Bypass -Command "$COMMAND | Write-Output"
  error-on-bad-return-code	

  ## squirrel makes an MSI
  ## but the MSI seems to do nothing, so lets delete it
  remove $GENERATED_INSTALLER_PATH\setup.msi

  ## move setup.exe as we have two setup files (one a exe one a zip)
  Move-Item -Path $GENERATED_INSTALLER_PATH\setup.exe -Destination $GENERATED_INSTALLER_PATH\application-setup.exe

  echo "squirrel-ed package!"
}


function test-vstest {

  echo "vstest-ing"
  $UNIT_TEST_DLL="$PROJECT_DIRECTORY_UNIT_TESTS\bin\$MS_BUILD_CONFIG\UnitTests.dll"

  if (!(Test-Path $UNIT_TEST_DLL )) {
    echo "UnitTests.dll missing! ... path $UNIT_TEST_DLL"
    exit 1
  }

  vstest.console.exe $UNIT_TEST_DLL --ListTests
  echo ""

  vstest.console.exe $UNIT_TEST_DLL $PROJECT_DIRECTORY_UNIT_TESTS\bin\$MS_BUILD_CONFIG\csfml-Window.dll
  
  if ( $LASTEXITCODE -ne 0) {
    echo "vstest failed"
    exit $LASTEXITCODE 
  }

   

  ##manualBuild\NUnit.Console-3.13.0\bin\net35\nunit3-console.exe NunitTests\bin\Release\net461\NunitTests.dll 
  ##if ( $LASTEXITCODE -ne 0) {
  ##  echo "nunit tests failed"
  ##  exit $LASTEXITCODE 
  ##}

  echo "vstest done"
}


function update-assembly-info {
  manualBuild\c-sharp-assembly-info-util\AssemblyInfoUtil.exe -set:$VERSION "$PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\Properties\AssemblyInfo.cs"

  if ( $LASTEXITCODE -ne 0) {
    echo "AssemblyInfoUtil.ex failed"
    exit $LASTEXITCODE 
  }
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


main_exec

