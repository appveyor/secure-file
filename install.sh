#! /bin/bash

curl -fsSL -O https://github.com/appveyor/secure-file/releases/download/1.0.1/secure-file.zip
unzip -q -o secure-file.zip -d appveyor-tools
chmod +x ./appveyor-tools/secure-file
rm secure-file.zip
