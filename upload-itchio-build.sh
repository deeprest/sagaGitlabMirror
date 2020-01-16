#!/bin/bash

# --if-changed 
macos=$(ls -td build/MacOS/Saga* | head -1)
butler push --verbose --fix-permissions $macos deeprest/saga2020:macos

linux=$(ls -td build/Linux/Saga* | head -1)
butler push --fix-permissions $linux deeprest/saga2020:linux

windows=$(ls -td build/Windows/Saga* | head -1)
butler push $windows deeprest/saga2020:windows

# zipwebgl=saga2020-webgl.zip
# rm $zipwebgl
# webgl=$(ls -td build/WebGL/Saga* | head -1)
# echo $webgl
# ditto -c -k --sequesterRsrc $webgl $zipwebgl
# butler push $zipwebgl deeprest/saga2020:webgl
