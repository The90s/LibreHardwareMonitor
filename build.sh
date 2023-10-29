#!/usr/bin/env bash

case "$1" in
r | release | Release)
    dotnet.exe build --configuration Release ./LibreHardwareMonitor.sln /target:LibreHardwareMonitorLib:rebuild /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary

    echo
    echo "Build Release version"
    echo
    if [[ $? != 0 ]]; then
        mv ./bin/Release/net472/* ../electron-vite/lib/
    fi
    ;;
*)
    dotnet.exe build ./LibreHardwareMonitor.sln /target:LibreHardwareMonitorLib:rebuild /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary

    echo
    echo "Build Debug version"
    echo
    if [[ $? != 0 ]]; then
        mv ./bin/Debug/net472/* ../electron-vite/lib/
    fi
    ;;
esac
