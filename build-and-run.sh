#!/bin/bash

## stops the execution of the script if a command or pipeline has an error
set -e
echo BUILD
sh ./build.sh
echo 

echo RUN
sh ./run.sh