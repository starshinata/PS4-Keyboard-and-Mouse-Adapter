#!/bin/bash

## cause i keep forgetting to write powershell infront of the ps1 file

## be error tollerant for process killing
set +e

echo "killing RemotePlay"
## documentation says /im but mingw no likey
taskkill.exe -im RemotePlay* -f


## stops the execution of the script if a command or pipeline has an error
set -e

powershell ./build.ps1
