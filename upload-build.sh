#!/bin/bash

zipfile=saga2020.app.zip
rm $zipfile
# push latest build to itch.io
# ditto is a macos anomaly
# pushd build/MacOS
app=$(ls -td build/MacOS/Saga* | head -1)
echo $app
# popd
ditto -c -k --sequesterRsrc --keepParent $app $zipfile
butler push $zipfile deeprest/saga2020:macos
