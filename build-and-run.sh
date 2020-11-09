#!/bin/bash


## be error tollerant for process killing
set +e
echo "killing RemotePlay"
## documentation says /im but mingw no likey
taskkill.exe -im RemotePlay* -f


## stops the execution of the script if a command or pipeline has an error
set -e

powershell ./build.ps1


BINARY_FOLDER="PS4KeyboardAndMouseAdapter/bin/Release"
cp manualBuild/my-mappings.json $BINARY_FOLDER/mappings.json
cd $BINARY_FOLDER


echo "EXECUTING ./PS4KeyboardAndMouseAdapter.exe"
./PS4KeyboardAndMouseAdapter.exe