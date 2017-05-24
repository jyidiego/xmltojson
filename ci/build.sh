#!/bin/bash

pushd ./xmltojson/src/api
    dotnet restore
    dotnet publish -o publish  -f netcoreapp1.1 -r ubuntu.14.04-x64
    ret_code=$?
    if [ $ret_code -eq 0 ]; then
        mv publish/* ../../../publish
    fi
popd
exit $ret_code
