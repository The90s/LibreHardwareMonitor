#!/usr/bin/env bash

BUILD_TYPE=Debug

rm -rf bin/

case "$1" in
r | release | Release)
    dotnet.exe build --configuration Release ./LibreHardwareMonitor.sln /target:LibreHardwareMonitorLib:rebuild /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary

    BUILD_TYPE=Release
    ;;
*)
    dotnet.exe build --configuration Debug ./LibreHardwareMonitor.sln /target:LibreHardwareMonitorLib:rebuild /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary

    BUILD_TYPE=Debug
    ;;
esac

echo
echo "Build ${BUILD_TYPE} version"
echo

if [[ $? == 0 ]]; then
    [[ ! -d "../electron-vite/lib" ]] && mkdir "../electron-vite/lib"
    echo "Copy LibreHardwareMonitor.dll [./bin/${BUILD_TYPE}/net472/*] to electron-vite Project [../electron-vite/lib/]"
    cp ./bin/${BUILD_TYPE}/net472/* ../electron-vite/lib/

    [[ ! -d "../electron-vite/lib" ]] && mkdir "../electron-vite-icicleflow/lib"
    echo "Copy LibreHardwareMonitor.dll [./bin/${BUILD_TYPE}/net472/*] to electron-vite-icicleflow Project [../electron-vite/lib/]"
    cp ./bin/${BUILD_TYPE}/net472/* ../electron-vite-icicleflow/lib/
fi
