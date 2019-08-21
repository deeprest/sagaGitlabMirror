#!/bin/bash

# unity build

# push to itch.io
# ditto is a macos anomaly
ditto -c -k --sequesterRsrc --keepParent SagaCity.app SagaCity.app.zip
butler push SagaCity.app.zip deeprest/sagacity:macos
