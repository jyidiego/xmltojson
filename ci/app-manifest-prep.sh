#!/bin/bash

set -xe


echo -e "applications: \n- name: xmltojson \n  memory: 1024M \n  host: xmltojson \n  path: /tmp/build/put/xmltojson-release/" > app-manifest-output/manifest.yml
