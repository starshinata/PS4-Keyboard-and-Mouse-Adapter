##
## USAGE
##     powershell build.ps1
##
## USAGE ARGS
##
##     -execGenerateArtefact=FALSE  - DO NOT generate the artefacts
##     -execGenerateArtefact=TRUE   - generate the artefacts (ZIPs and EXEs)
##                                    If flag 'execGenerateArtefact' is omitted this we default to TRUE
##
##     -execTest=FALSE              - DO NOT run units tests
##     -execTest=TRUE               - run unit tests
##                                    If flag 'execTest' is omitted this we default to TRUE
##
##


## param needs to be first non comment line of file
param ([string]$execGenerateArtefact = 'TRUE', [string]$execTest = 'TRUE')

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


## at top as you must define a function before calling it
function error-if-path-does-not-exist {
    $PATH = $args[0]
    if (!(Test-Path -Path $PATH)) {
        echo "Path doesn't exist. '$PATH'"
        exit 99
    }
}


################################
################################

## might need configuring
$CERT_DIRECTORY = "E:\workspace\##certificates\github.com-pancakeslp\"
error-if-path-does-not-exist $CERT_DIRECTORY

#$MS_BUILD_CONFIG="Debug"
$MS_BUILD_CONFIG = "Release"

$VERSION = "4.0.0"

################################
################################
$VISUAL_STUDIO_PATH = "C:\Program Files\Microsoft Visual Studio\2022\Community\"
error-if-path-does-not-exist $VISUAL_STUDIO_PATH

## Path for MSBuild.exe
$env:Path += ";$VISUAL_STUDIO_PATH\MSBuild\Current\Bin\amd64\"

## Path for signtool.exe
$env:Path += ";C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x64\"

## Path for vstest.console.exe
$env:Path += ";$VISUAL_STUDIO_PATH\Common7\IDE\CommonExtensions\Microsoft\TestWindow\"

################################
################################

## get location of script
$scriptPath = $MyInvocation.MyCommand.Path
$scriptDirectory = Split-Path $scriptPath

## cd location of script
Set-Location -Path $scriptDirectory

## now PWD should be absolute location of GIT REPO
$DIRECTORY_REPO_ROOT_ABSOLUTE = $pwd

################################
################################

$DIRECTORY_WIP_INSTALLERS_COMMON = "temp\common\"
$DIRECTORY_WIP_INSTALLERS_NUGET = "temp\nuget\"
$DIRECTORY_WIP_INSTALLERS_ZIP = "temp\zip\"
$DIRECTORY_RELEASE = "ReleaseArtefacts\"

$PROJECT_DIRECTORY_COMMON = "code\common\"
$PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER = "code\PS4KeyboardAndMouseAdapter\"
$PROJECT_DIRECTORY_PS4_REMOTE_PLAY_INJECTION = "code\PS4RemotePlayInjection\"
$PROJECT_DIRECTORY_UNIT_TESTS = "code\UnitTests\"

################################
################################

$FILE_NUGET_SPEC_SOURCE = "manualBuild\nuget\PS4KeyboardAndMouseAdapter.nuspec.template.xml"
$FILE_NUGET_SPEC_TARGET = "manualBuild\nuget\PS4KeyboardAndMouseAdapter.nuspec"
$FILE_PS4KMA_NUPKG = "PS4KeyboardAndMouseAdapter.${VERSION}.nupkg"

$NUGET_PACKAGE_PATH = "${env:USERPROFILE}\.nuget\packages\"
$EXE_SQUIRREL = "${NUGET_PACKAGE_PATH}\clowd.squirrel\2.9.42\tools\Squirrel.exe"

################################
################################

$DOT_NET_BUILD_TARGET = "net8.0-windows7.0"

$BIN_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER = "$PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\bin\$MS_BUILD_CONFIG\$DOT_NET_BUILD_TARGET\"
$BIN_DIRECTORY_UNIT_TESTS = "$PROJECT_DIRECTORY_UNIT_TESTS\bin\$MS_BUILD_CONFIG\$DOT_NET_BUILD_TARGET\"

################################
################################

## print env variables
dir env:
echo ""

################################
################################

function add-build-date {
    make-dir $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\Resources\

    ## "-Format s" means sortable aka ISO 8601
    $DATETIME = (get-date -Format s)
    echo $DATETIME > $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\Resources\BuildDate.txt
}


function build-msbuild {

    echo "@@@ msbuild-ing"

    ## "-p:UseSharedCompilation=false" for CodeQL
    MSBuild.exe PS4KeyboardAndMouseAdapter.sln `
        -p:Configuration=$MS_BUILD_CONFIG      `
        -p:UseSharedCompilation=false          `
        -p:VersionNumber=$VERSION

    if ( $LASTEXITCODE -ne 0) {
        echo "msbuild failed"
        exit $LASTEXITCODE
    }

    echo "@@@ msbuild sleep"
    ## sleep cause sometimes this step returns too early
    ## wait to make sure nothing errors
    Start-Sleep -Milliseconds 500

    echo "@@@ msbuild done"
    echo ""
}


function cleanup-prebuild {
    echo "@@@ cleanup-prebuild running "

    remove $DIRECTORY_RELEASE

    remove $DIRECTORY_WIP_INSTALLERS_COMMON
    remove $DIRECTORY_WIP_INSTALLERS_NUGET
    remove $DIRECTORY_WIP_INSTALLERS_ZIP

    remove $GENERATED_INSTALLER_PATH

    remove PS4KeyboardAndMouseAdapter.*.nupkg

    remove $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\bin\
    remove $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\logs\
    remove $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\obj\

    remove $PROJECT_DIRECTORY_PS4_REMOTE_PLAY_INJECTION\bin\
    remove $PROJECT_DIRECTORY_PS4_REMOTE_PLAY_INJECTION\obj\

    remove temp
    remove TestResults

    echo "@@@ cleanup-prebuild ran "
    echo ""
}


function cleanup-postbuild {
    echo "@@@ cleanup-postbuild running"

    remove PS4KeyboardAndMouseAdapter.*.nupkg
    remove temp
    remove TestResults

    echo "@@@ cleanup-postbuild ran "
    echo ""
}


function copy-non-cs-project-files {
    ## we are copying things that dont live in a cs project
    ## we are in solution root encase you are wondering
    Copy-Item                    profiles\default-profile.json   $BIN_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\profile-previous.json
    Copy-Item  -recurse -Force   profiles                        $BIN_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\profiles
}


function dependencies-nuget {
    echo "@@@ dependencies-nuget running"

    ## this was a nuget command when using packages.config
    ## now we use dotnet for dependencies defined via "packageref"
    dotnet restore
    error-on-bad-return-code

    echo "@@@ dependencies-nuget ran"
    echo ""
}


function error-on-bad-return-code {
    if ($LASTEXITCODE -ne 0) {
        echo "LASTEXITCODE '$LASTEXITCODE'"
        echo "error-on-bad-return-code!"
        exit $LASTEXITCODE
    }
}


function generate-artefact-release {

    if ($execGenerateArtefact -eq "TRUE" -And $MS_BUILD_CONFIG -eq "Release") {

        echo "@@@ generate-artefact-release running"
        echo ""

        copy-non-cs-project-files
        make-installer-common
        sign-executables

        ## pancakeslp 2023.04.08
        ## if you run `make-installer-zip` then `make-installer-exe` zip size is about 18mb
        ## if you run `make-installer-exe` then `make-installer-zip` zip size is about 70mb
        ## extra size is because it will include nuget stuff
        ## TODO fix this

        make-installer-exe
        make-installer-zip

        echo "@@@ generate-artefact-release ran"

    }
    else {
        echo "@@@ generate-artefact-release SKIPPED, because "

        if (-Not $execGenerateArtefact -eq "TRUE") {
            echo "... arg execGenerateArtefact was '$execGenerateArtefact'"
        }

        if (-Not $MS_BUILD_CONFIG -eq "Release") {
            echo "... arg MS_BUILD_CONFIG was '$MS_BUILD_CONFIG'"
        }
    }

    echo ""
}


## see top of file
## function error-if-path-does-not-exist {


function main_exec {

    add-build-date

    cleanup-prebuild

    dependencies-nuget

    validate-xaml-xmllint

    update-assembly-info

    build-msbuild

    ##TODO
    #test-vstest

    generate-artefact-release

    ##TODO
    ##cleanup-postbuild

    echo ""
    echo "build.ps1 SUCCESS!"
}


function make-dir {
    $PATH = $args[0]

    if (!(Test-Path $PATH)) {
        New-Item -ItemType directory -Path $PATH
    }
}

function make-installer-common {

    echo ""
    echo "@@@ make-installer-common running"

    make-dir $DIRECTORY_WIP_INSTALLERS_COMMON
    Copy-Item  -Force -Recurse   $BIN_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\*   $DIRECTORY_WIP_INSTALLERS_COMMON

    echo "@@@ make-installer-common ran"
}


function make-installer-exe {
    echo ""
    echo "@@@ make-installer-exe running"

    make-dir $DIRECTORY_RELEASE
    make-dir $DIRECTORY_WIP_INSTALLERS_NUGET


    make-installer-nuget
    make-installer-nuget-to-exe
    sign-installer-exe
    echo "@@@ make-installer-exe ran"
}


function make-installer-nuget {
    echo ""
    echo "@@@ make-installer-nuget running"

    error-if-path-does-not-exist  $DIRECTORY_WIP_INSTALLERS_COMMON
    error-if-path-does-not-exist  $DIRECTORY_WIP_INSTALLERS_NUGET

    Copy-Item  -Force -Recurse  ${DIRECTORY_WIP_INSTALLERS_COMMON}\*  ${DIRECTORY_WIP_INSTALLERS_NUGET}

    make-nuget-spec

    nuget pack $FILE_NUGET_SPEC_TARGET
    error-on-bad-return-code

    echo "@@@ make-installer-nuget ran"
}


function make-installer-nuget-to-exe {
    echo ""
    echo "@@@ make-installer-nuget-to-exe running ..."

    ## arg --allowUnaware is because it isnt detecting "SquirrelAwareVersion" in code\PS4KeyboardAndMouseAdapter\app.manifest
    $COMMAND = " $EXE_SQUIRREL releasify --package=$FILE_PS4KMA_NUPKG  --releaseDir=$DIRECTORY_RELEASE --framework net8.0"
    echo "command"
    echo "  $COMMAND"
    echo ""

    powershell.exe -ExecutionPolicy Bypass -Command "$COMMAND | Write-Output"
    error-on-bad-return-code

    ## this is a duplicate file, remove it
    remove $FILE_PS4KMA_NUPKG
    error-on-bad-return-code

    ## squirrel makes an MSI
    ## but the MSI seems to do nothing, so lets delete it
    ##TODO
    #remove $DIRECTORY_WIP_INSTALLERS_COMMON\setup.msi

    ## move setup.exe as we have two setup files (one a exe one a zip)
    Move-Item -Path $DIRECTORY_RELEASE\PS4KeyboardAndMouseAdapterSetup.exe -Destination $DIRECTORY_RELEASE\application-setup.exe

    echo "@@@ make-installer-nuget-to-exe ran"
}


function make-installer-zip {

    echo ""
    echo "@@@ make-installer-zip running"


    make-dir $DIRECTORY_RELEASE
    make-dir $DIRECTORY_WIP_INSTALLERS_ZIP
    make-dir $DIRECTORY_WIP_INSTALLERS_ZIP\app-$VERSION


    Copy-Item  -Force -Recurse  $DIRECTORY_WIP_INSTALLERS_COMMON\* $DIRECTORY_WIP_INSTALLERS_ZIP\app-$VERSION
    Copy-Item  -Force           manualBuild\extract-me-bin\*       $DIRECTORY_WIP_INSTALLERS_ZIP

    $compress = @{
        Path            = "$DIRECTORY_WIP_INSTALLERS_ZIP\*"
        DestinationPath = "$DIRECTORY_RELEASE\application-extract-me.zip"
    }
    Compress-Archive @compress
    error-on-bad-return-code

    ##TODO
    ##remove $EXTRACTED_PATH

    echo "@@@ make-installer-zip ran"
    echo ""
}


function make-nuget-spec {

    $FIND = "<version>REPLACE_VERSION_REPLACE</version>"
    $REPLACE = "<version>$VERSION</version>"

    remove $FILE_NUGET_SPEC_TARGET

    Copy-Item $FILE_NUGET_SPEC_SOURCE -Destination $FILE_NUGET_SPEC_TARGET

    ((Get-Content -path $FILE_NUGET_SPEC_TARGET -Raw) -replace $FIND, $REPLACE) | Set-Content -Path $FILE_NUGET_SPEC_TARGET
}


function manually-sign-file {
    $FILE_NAME = $args[0]

    ## type is the windows equivalent of cat
    $CERT_PASSWORD = $( type ${CERT_DIRECTORY}cert-password.txt )
    $CERT_PFX = "${CERT_DIRECTORY}github.com-pancakeslp.pfx"

    if (![System.IO.File]::Exists($FILE_NAME)) {
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

    if (($FILE_NAME) -And (Test-Path $FILE_NAME)) {
        Remove-Item -Recurse $FILE_NAME
    }
}


function sign-executables {
    echo ""
    echo "@@@ sign-executables running"

    manually-sign-file  "$BIN_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\PS4KeyboardAndMouseAdapter.exe"
    echo "@@@ sign-executables ran"
}


function sign-installer-exe {
    echo ""
    echo "@@@ sign-installer-exe running"

    manually-sign-file  $DIRECTORY_RELEASE\application-setup.exe
    echo "@@@ sign-installer-exe ran"
}


function test-vstest {

    if (!($execTest -eq "TRUE")) {
        echo "@@@ test-vstest SKIPPED, because arg execTest was '$execTest'"
    }

    echo "@@@ test-vstest running"
    $UNIT_TESTS_DLL = "$BIN_DIRECTORY_UNIT_TESTS\UnitTests.dll"

    if (!(Test-Path $UNIT_TESTS_DLL)) {
        echo "UnitTests.dll missing! ... path $UNIT_TESTS_DLL"
        exit 1
    }


    vstest.console.exe $UNIT_TESTS_DLL --ListTests
    echo ""

    vstest.console.exe $UNIT_TESTS_DLL       `
        "--Framework:.NETCoreApp,Version=v8.0" `
        /Platform:x64

    if ($LASTEXITCODE -ne 0) {
        echo "vstest failed"
        exit $LASTEXITCODE
    }



    ##manualBuild\NUnit.Console-3.13.0\bin\net35\nunit3-console.exe NunitTests\bin\Release\net461\NunitTests.dll
    ##if ( $LASTEXITCODE -ne 0) {
    ##  echo "nunit tests failed"
    ##  exit $LASTEXITCODE
    ##}

    echo "@@@ test-vstest ran"
    echo ""
}


function update-assembly-info {
    manualBuild\c-sharp-assembly-info-util\AssemblyInfoUtil.exe -set:$VERSION "$PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER\Properties\AssemblyInfo.cs"

    if ( $LASTEXITCODE -ne 0) {
        echo "AssemblyInfoUtil.ex failed"
        exit $LASTEXITCODE
    }
}


function validate-xaml-xmllint {
    echo ""
    echo "@@@ validate-xaml-xmllint  running"

    $files = Get-ChildItem -recurse *.xaml | where { !$_.PSIsContainer }

    foreach ($file in $files) {

        ## here we are using xmllint, which is provided by libxml
        ## if you need to install it, use the chocolatey command
        ##```
        ##  choco install xsltproc
        ##```
        ##
        ## if you dont know chocolatey,
        ## then see https://chocolatey.org/
        ##
        ## if you dont know xmllint,
        ## then see https://gnome.pages.gitlab.gnome.org/libxml2/xmllint.html
        xmllint.exe $file.FullName  --noout

        ## if you get a negative exit coder assume the binary is either corrupted or missing DLLs
        if ( $LASTEXITCODE -ne 0) {
            echo "ERROR - xmllint exit code '$LASTEXITCODE' for file '$file'"
            exit $LASTEXITCODE 
        }

    }

    echo "@@@ valid-xaml-xmllint ran"
}


################################
################################


main_exec
