#!/usr/bin/env bash
set -euo pipefail

export DOTNET_USE_POLLING_FILE_WATCHER=1
dotnet watch --project projem.csproj run
