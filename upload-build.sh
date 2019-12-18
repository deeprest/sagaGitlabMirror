#!/bin/bash

# unity build

# push to itch.io
# ditto is a macos anomaly
ditto -c -k --sequesterRsrc --keepParent saga2020.app saga2020.app.zip
butler push saga2020.app.zip deeprest/saga2020:macos
