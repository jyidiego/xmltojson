#!/bin/bash

pushd ./xmltocsv/test/api
    dotnet restore
    dotnet test
    ret_code=$?
popd
exit $ret_code
