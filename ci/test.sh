#!/bin/bash

pushd ./xmltojson/test/api
    dotnet restore
    dotnet test
    ret_code=$?
popd
exit $ret_code
