#!/bin/bash

git lfs ls-files | grep -E -o [_0-9A-Za-z\/\.-]+$ | while read -r line; do chmod +w $line; done
exit
