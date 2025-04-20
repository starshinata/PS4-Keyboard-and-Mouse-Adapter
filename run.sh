#!/bin/bash


## be error tolerant for process killing
set +e


echo "REMINDER this is to RUN, not BUILD"
echo "REMINDER this is to RUN, not BUILD"
echo ""


echo "killing RemotePlay"
## documentation says /im but mingw no likey
taskkill.exe -im RemotePlay* -f


## stops the execution of the script if a command or pipeline has an error
set -e

DOTNET_BUILD_TARGET="/net8.0-windows7.0/"
BINARY_FOLDER="code/PS4KeyboardAndMouseAdapter/bin/Debug/$DOTNET_BUILD_TARGET/"
BINARY_FOLDER="code/PS4KeyboardAndMouseAdapter/bin/Release/$DOTNET_BUILD_TARGET/"

cp profiles/pancakes-destiny-profile.json $BINARY_FOLDER/profile-previous.json

echo "if you want to see the logs"
echo " tail -f  $PWD/$BINARY_FOLDER/logs/log.txt "


cd $BINARY_FOLDER

echo "EXECUTING ./PS4KeyboardAndMouseAdapter.exe"
echo ""
./PS4KeyboardAndMouseAdapter.exe