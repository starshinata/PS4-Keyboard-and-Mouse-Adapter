#!/bin/bash


PROJECT_DIRECTORY_COMMON="code\common"
PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER="code\PS4KeyboardAndMouseAdapter"
PROJECT_DIRECTORY_PS4_REMOTE_PLAY_INJECTION="code\PS4RemotePlayInjection"
PROJECT_DIRECTORY_UNIT_TESTS="code\UnitTests"

echo "nupkg"
rm -rf PS4KeyboardAndMouseAdapter.*.nupkg

echo "common"
rm -rf PROJECT_DIRECTORY_COMMON/bin
rm -rf PROJECT_DIRECTORY_COMMON/obj

echo "ps4 kma"
rm -rf $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER/bin
rm -rf $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER/logs
rm -rf $PROJECT_DIRECTORY_PS4_KEYBOARD_AND_MOUSE_ADAPTER/obj

echo "ps4 remoteplay injection"
rm -rf $PROJECT_DIRECTORY_PS4_REMOTE_PLAY_INJECTION/bin
rm -rf $PROJECT_DIRECTORY_PS4_REMOTE_PLAY_INJECTION/obj

echo "unit test"
rm -rf PROJECT_DIRECTORY_UNIT_TESTS/bin
rm -rf PROJECT_DIRECTORY_UNIT_TESTS/obj

echo "packages"
## yes there are two package folders
## nuget and msbuild dont want to play nice
## this might change if we move to .net core
rm -rf code/packages
rm -rf packages
