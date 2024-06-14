#!/bin/bash

## cause i keep forgetting to write powershell infront of the ps1 file

## be error tolerant for process killing
set +e

## documentation says ` taskkill /im ` but mingw no likey

echo "killing RemotePlay"
taskkill.exe -im "RemotePlay*" -f

echo "killing PS4KeyboardAndMouseAdapter"
taskkill.exe -im "PS4KeyboardAndMouseAdapter*" -f

## stops the execution of the script if a command or pipeline has an error
set -e

powershell ./build.ps1 
#powershell ./build.ps1 -$execTest FALSE -execGenerateArtefact FALSE
