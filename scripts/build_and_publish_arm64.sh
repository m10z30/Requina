#!/bin/bash
set -e

# Build
dotnet publish -c Release -r osx-arm64 --self-contained true \
  /p:PublishSingleFile=true \
  /p:IncludeAllContentInSingleFile=true \
  /p:EnableCompressionInSingleFile=true

# Copy to /usr/local/bin
APP_NAME="requina"
PUBLISH_DIR="bin/Release/net8.0/osx-arm64/publish"
sudo cp "$PUBLISH_DIR/Requina" "/usr/local/bin/$APP_NAME"
sudo chmod +x "/usr/local/bin/$APP_NAME"

echo "✅ Installed $APP_NAME. Run it with: $APP_NAME"
