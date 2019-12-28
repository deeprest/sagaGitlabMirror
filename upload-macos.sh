#!/bin/bash

zipmacos=saga2020-macos.zip
rm $zipmacos
macos=$(ls -td build/MacOS/Saga* | head -1)
echo $macos
ditto -c -k --sequesterRsrc --keepParent $macos $zipmacos
butler push $zipmacos deeprest/saga2020:macos
