#!/bin/bash

cd code

function check-project {
    PROJECT=$1
    echo $PROJECT
    cd $PROJECT
    dotnet list package --outdated
    cd ../
}

## only need one ATM as they share dependencies
check-project Common/
#check-project PS4KeyboardAndMouseAdapter/
#check-project PS4RemotePlayInjection/
#check-project TestTools/
#check-project UnitTests/
