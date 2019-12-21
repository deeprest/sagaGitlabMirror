#!/bin/bash

zipfile=saga2020-webgl.zip
rm $zipfile
# push latest build to itch.io
# ditto is a macos anomaly
# pushd build/MacOS
app=$(ls -td build/WebGL/Saga* | head -1)
echo $app
# popd
ditto -c -k --sequesterRsrc $app $zipfile
butler push $zipfile deeprest/saga2020:webgl
