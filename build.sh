#!/usr/bin/env bash
set -eu

dotnet restore
dotnet build

# Tests