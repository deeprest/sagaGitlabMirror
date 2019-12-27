#!/bin/bash

zipmacos=saga2020-macos.zip
rm $zipmacos
macos=$(ls -td build/MacOS/Saga* | head -1)
echo $macos
ditto -c -k --sequesterRsrc --keepParent $macos $zipmacos
butler push $zipmacos deeprest/saga2020:macos

ziplinux=saga2020-linux.zip
rm $ziplinux
linux=$(ls -td build/Linux/Saga* | head -1)
echo $linux
ditto -c -k --sequesterRsrc $linux $ziplinux
butler push $ziplinux deeprest/saga2020:linux

zipwindows=saga2020-windows.zip
rm $zipwindows
windows=$(ls -td build/Windows/Saga* | head -1)
echo $windows
ditto -c -k --sequesterRsrc $windows $zipwindows
butler push $zipwindows deeprest/saga2020:windows

zipwebgl=saga2020-webgl.zip
rm $zipwebgl
webgl=$(ls -td build/WebGL/Saga* | head -1)
echo $webgl
ditto -c -k --sequesterRsrc $webgl $zipwebgl
butler push $zipwebgl deeprest/saga2020:webgl
