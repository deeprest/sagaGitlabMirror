#!/bin/bash

zipmacos=saga2020-macos.zip
zipwebgl=saga2020-webgl.zip
rm $zipmacos
rm $zipwebgl
macos=$(ls -td build/MacOS/Saga* | head -1)
webgl=$(ls -td build/WebGL/Saga* | head -1)
echo $macos
echo $webgl
ditto -c -k --sequesterRsrc --keepParent $macos $zipmacos
ditto -c -k --sequesterRsrc $webgl $zipwebgl
butler push $zipmacos deeprest/saga2020:macos
butler push $zipwebgl deeprest/saga2020:webgl
